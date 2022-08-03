using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges) =>
       await FindByCondition(employee => employee.CompanyId.Equals(companyId), trackChanges)
            .OrderBy(employee => employee.Name)
            .ToListAsync();

    public Task<Employee?> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
       FindByCondition(employee => employee.CompanyId.Equals(companyId) && employee.Id.Equals(id), trackChanges)
            .SingleOrDefaultAsync();

    public void CreateEmployeeForCompany(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        Create(employee);
    }

    public void DeleteEmployee(Employee employee)
    {
        Delete(employee);
    }
}