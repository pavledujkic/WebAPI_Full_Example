using Microsoft.AspNetCore.HttpOverrides;
using NLog;
using System;
using WebAPI_Full_Example.Extensions;

namespace WebAPI_Full_Example
{
    public class Program
    {
        public static void Main(string[] args)
        {

            LogManager.LoadConfiguration("nlog.config");

            try
            {
                LogManager.LogFactory.GetCurrentClassLogger().Debug("Application Starting Up");

                var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

                builder.Services.ConfigureCors();
                builder.Services.ConfigureIISIntegration();
                builder.Services.ConfigureLoggerService();
                builder.Services.AddControllers();

                var app = builder.Build();

                // Configure the HTTP request pipeline
                if (app.Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseHsts();
                }
                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseCors("CorsPolicy");
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.All
                });
                app.UseRouting();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
                app.Run();
            }
            catch (Exception exception)
            {
                LogManager.LogFactory.GetCurrentClassLogger().Error(exception, "Stopped program because of exception: " + exception);
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}