using CompanyEmployees.ActionFilters;
using CompanyEmployees.Extensions;
using Contracts;
using Entities.DataTransferObjects;
using LoggerService;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Repository.DataShaping;

LogManager.LoadConfiguration("nlog.config");

try
{
    LogManager.GetCurrentClassLogger().Debug("Application Starting Up");

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.ConfigureCors();
    builder.Services.ConfigureIISIntegration();
    builder.Services.ConfigureLoggerService();
    builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.ConfigureSqlContext(builder.Configuration);
    builder.Services.ConfigureRepositoryManager();
    builder.Services.ConfigureServiceManager();
    builder.Services.AddControllers(config =>
    {
        config.RespectBrowserAcceptHeader = true;
        config.ReturnHttpNotAcceptable = true;
    }).AddNewtonsoftJson()
        .AddXmlDataContractSerializerFormatters()
        .AddCustomCSVFormatter();
    builder.Services.Configure<ApiBehaviorOptions>(options =>
        options.SuppressModelStateInvalidFilter = true);
    builder.Services.AddScoped<ValidationFilterAttribute>();
    builder.Services.AddScoped<ValidateCompanyExistsAttribute>();
    builder.Services.AddScoped<ValidateEmployeeForCompanyExistsAttribute>();
    builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();

    WebApplication app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
        app.UseDeveloperExceptionPage();
    else
        app.UseHsts();

    app.ConfigureExceptionHandler(new LoggerManager());
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.All
    });
    app.UseRouting();
    app.UseCors("CorsPolicy");
    app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
    
    app.Run();
}
catch (Exception exception)
{
    LogManager.GetCurrentClassLogger().Error(exception, "Stopped program because of exception: " + exception);
    throw;
}
finally
{
    LogManager.Shutdown();
}
