using Contracts;
using Entities;
using Entities.Models;

namespace Repository;

public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public IEnumerable<Company> GetAllCompanies(bool trackChanges)
    {
        return FindAll(trackChanges)
            .OrderBy(company => company.Name)
            .ToList();
    }

    public Company? GetCompany(Guid companyId, bool trackChanges) =>
        FindByCondition(company => company.Id.Equals(companyId), trackChanges)
            .SingleOrDefault();

    public void CreateCompany(Company company) => Create(company);
}