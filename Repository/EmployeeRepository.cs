using Contracts;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;

namespace Repository;

internal sealed class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId,
        EmployeeParameters? employeeParameters, bool trackChanges)
    {
        var employees = await FindByCondition(employee => employee.CompanyId.Equals(companyId)
                , trackChanges)
            //.FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
            //.Search(employeeParameters.SearchTerm)
            //.Sort(employeeParameters.OrderBy!)
            //.Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
            //.Take(employeeParameters.PageSize)
            .ToListAsync();

        //var count = await FindByCondition(e =>
        //    e.CompanyId.Equals(companyId), trackChanges).CountAsync();

        return employees;

        //return new PagedList<Employee>(employees, count, employeeParameters.PageNumber,
        //    employeeParameters.PageSize);
    }

    public Task<Employee?> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
       FindByCondition(employee => employee.CompanyId.Equals(companyId) && employee.Id.Equals(id), trackChanges)
            .SingleOrDefaultAsync();

    public void CreateEmployeeForCompany(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        Create(employee);
    }

    public void DeleteEmployee(Employee employee) => Delete(employee);
}