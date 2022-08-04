using Contracts;
using Entities;

namespace Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private CompanyRepository? _companyRepository;
    private EmployeeRepository? _employeeRepository;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }

    public ICompanyRepository Company
    {
        get { return _companyRepository ??= new CompanyRepository(_repositoryContext); }
    }

    public IEmployeeRepository Employee
    {
        get { return _employeeRepository ??= new EmployeeRepository(_repositoryContext); }
    }

    public Task SaveAsync() => _repositoryContext.SaveChangesAsync();
}