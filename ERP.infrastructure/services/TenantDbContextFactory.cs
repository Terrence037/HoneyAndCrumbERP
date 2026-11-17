using ERP.infrastructure.data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ERP.infrastructure.services;

public interface ITenantDbContextFactory
{
    Task<TenantErpDbContext> CreateDbContextAsync(int companyId, CancellationToken cancellationToken = default);
}

public class TenantDbContextFactory : ITenantDbContextFactory
{
    private readonly ITenantDatabaseResolver _resolver;
    private readonly IConfiguration _configuration;

    public TenantDbContextFactory(ITenantDatabaseResolver resolver, IConfiguration configuration)
    {
        _resolver = resolver;
        _configuration = configuration;
    }

    public async Task<TenantErpDbContext> CreateDbContextAsync(int companyId, CancellationToken cancellationToken = default)
    {
        var dbInfo = await _resolver.ResolveAsync(companyId, cancellationToken)
            ?? throw new InvalidOperationException($"No active database configuration found for CompanyId {companyId}.");

        var userId = _configuration[$"TenantCredentials:{dbInfo.CredentialKey}:UserId"];
        var password = _configuration[$"TenantCredentials:{dbInfo.CredentialKey}:Password"];

        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException($"Credentials missing for CredentialKey '{dbInfo.CredentialKey}'.");
        }

        var connBuilder = new SqlConnectionStringBuilder
        {
            DataSource = dbInfo.ServerName,
            InitialCatalog = dbInfo.DatabaseName,
            UserID = userId,
            Password = password,
            Encrypt = true,
            TrustServerCertificate = true,
            ConnectTimeout = 60
        };

        var optionsBuilder = new DbContextOptionsBuilder<TenantErpDbContext>();
        optionsBuilder.UseSqlServer(connBuilder.ConnectionString, sql => sql.EnableRetryOnFailure());

        return new TenantErpDbContext(optionsBuilder.Options);
    }
}