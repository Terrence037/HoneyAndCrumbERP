using ERP.infrastructure.data;
using Microsoft.EntityFrameworkCore;

namespace ERP.infrastructure.services;

public interface ITenantDatabaseResolver
{
    Task<TenantDatabaseInfo?> ResolveAsync(int companyId, CancellationToken cancellationToken = default);
}

public class TenantDatabaseResolver : ITenantDatabaseResolver
{
    private readonly MasterErpDbContext _masterDb;

    public TenantDatabaseResolver(MasterErpDbContext masterDb)
    {
        _masterDb = masterDb;
    }

    public async Task<TenantDatabaseInfo?> ResolveAsync(int companyId, CancellationToken cancellationToken = default)
    {
        return await _masterDb.CompanyDatabases
            .AsNoTracking()
            .Where(cd => cd.CompanyId == companyId && cd.IsActive && cd.Company.IsActive)
            .Select(cd => new TenantDatabaseInfo
            {
                ServerName = cd.ServerName,
                DatabaseName = cd.DatabaseName,
                CredentialKey = cd.CredentialKey
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}