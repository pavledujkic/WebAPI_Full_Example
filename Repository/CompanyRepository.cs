using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

internal sealed class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
    {
    }

    public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges) =>
        await FindAll(trackChanges)
            .OrderBy(company => company.Name)
            .ToListAsync();

    public Task<Company?> GetCompanyAsync(Guid companyId, bool trackChanges) =>
        FindByCondition(company => company.Id.Equals(companyId), trackChanges)
            .SingleOrDefaultAsync();

    public void CreateCompany(Company company) => Create(company);

    public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges) =>
        await FindByCondition(company => ids.Contains(company.Id), trackChanges)
            .ToListAsync();

    public void DeleteCompany(Company company) => Delete(company);
}