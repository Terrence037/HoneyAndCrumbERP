using ERP.domain.entities;
using ERP.infrastructure.data;
using ERP.infrastructure.services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MasterErpDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MasterErp"),
        sql => sql.EnableRetryOnFailure()));

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<MasterErpDbContext>();

builder.Services.AddDbContext<TenantErpDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TenantErp")));

builder.Services.AddOpenApi();

builder.Services.AddScoped<ITenantDatabaseResolver, TenantDatabaseResolver>();
builder.Services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/test-tenant/{companyId:int}", async (int companyId, ITenantDbContextFactory tenantFactory) =>
{
    await using var db = await tenantFactory.CreateDbContextAsync(companyId);
    var productCount = await db.Products.CountAsync();
    return Results.Ok(new { companyId, productCount });
});

// -------------------- MASTER DB ENDPOINTS --------------------

app.MapGet("/companies", async (MasterErpDbContext masterDb) =>
{
    var list = await masterDb.Companies.AsNoTracking().OrderBy(c => c.CompanyId).ToListAsync();
    return Results.Ok(list);
});

app.MapPost("/companies", async (CreateCompanyRequest req, MasterErpDbContext masterDb) =>
{
    var exists = await masterDb.Companies.AnyAsync(c => c.CompanyCode == req.CompanyCode);
    if (exists)
        return Results.Conflict(new { message = $"CompanyCode '{req.CompanyCode}' already exists." });

    var company = new Company
    {
        CompanyCode = req.CompanyCode.Trim(),
        CompanyName = req.CompanyName.Trim(),
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    masterDb.Companies.Add(company);
    await masterDb.SaveChangesAsync();

    return Results.Created($"/companies/{company.CompanyId}", new
    {
        company.CompanyId,
        company.CompanyCode,
        company.CompanyName,
        company.SubscriptionPlan,
        company.IsActive,
        company.CreatedAt
    });
});

app.MapPost("/devices", async (CreateDeviceRequest req, MasterErpDbContext masterDb) =>
{
    var companyExists = await masterDb.Companies.AnyAsync(c => c.CompanyId == req.CompanyId);
    if (!companyExists)
        return Results.NotFound(new { message = $"Company {req.CompanyId} not found." });

    var duplicate = await masterDb.Devices.AnyAsync(d =>
        d.CompanyId == req.CompanyId && d.DeviceCode == req.DeviceCode);
    if (duplicate)
        return Results.Conflict(new { message = $"DeviceCode '{req.DeviceCode}' already exists for this company." });

    var device = new Device
    {
        CompanyId = req.CompanyId,
        DeviceCode = req.DeviceCode.Trim(),
        DeviceName = req.DeviceName.Trim(),
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    masterDb.Devices.Add(device);
    await masterDb.SaveChangesAsync();

    return Results.Created($"/devices/{device.DeviceId}", new
    {
        device.DeviceId,
        device.CompanyId,
        device.DeviceCode,
        device.DeviceName,
        device.IsActive
    });
});

app.MapPost("/company-databases", async (CreateCompanyDatabaseRequest req, MasterErpDbContext masterDb) =>
{
    var companyExists = await masterDb.Companies.AnyAsync(c => c.CompanyId == req.CompanyId);
    if (!companyExists)
        return Results.NotFound(new { message = $"Company {req.CompanyId} not found." });

    var companyDb = new CompanyDatabase
    {
        CompanyId = req.CompanyId,
        ServerName = req.ServerName.Trim(),
        DatabaseName = req.DatabaseName.Trim(),
        CredentialKey = req.CredentialKey.Trim(),
        IsActive = true
    };

    masterDb.CompanyDatabases.Add(companyDb);
    await masterDb.SaveChangesAsync();

    return Results.Created($"/company-databases/{companyDb.CompanyDatabaseId}", new
    {
        companyDb.CompanyDatabaseId,
        companyDb.CompanyId,
        companyDb.ServerName,
        companyDb.DatabaseName,
        companyDb.CredentialKey,
        companyDb.IsActive
    });
});

// -------------------- TENANT DB ENDPOINTS --------------------

app.MapGet("/tenant/{companyId:int}/products", async (int companyId, ITenantDbContextFactory factory) =>
{
    await using var db = await factory.CreateDbContextAsync(companyId);
    var items = await db.Products.AsNoTracking().OrderBy(p => p.ProductName).ToListAsync();
    return Results.Ok(items);
});

app.MapPost("/tenant/{companyId:int}/products", async (int companyId, CreateProductRequest req, ITenantDbContextFactory factory) =>
{
    await using var db = await factory.CreateDbContextAsync(companyId);

    var exists = await db.Products.AnyAsync(p => p.ProductCode == req.ProductCode);
    if (exists)
        return Results.Conflict(new { message = $"ProductCode '{req.ProductCode}' already exists." });

    var product = new Product
    {
        ProductCode = req.ProductCode.Trim(),
        ProductName = req.ProductName.Trim(),
        Category = req.Category.Trim(),
        UnitPrice = req.UnitPrice,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/tenant/{companyId}/products/{product.ProductId}", product);
});

app.MapGet("/tenant/{companyId:int}/customers", async (int companyId, ITenantDbContextFactory factory) =>
{
    await using var db = await factory.CreateDbContextAsync(companyId);
    var items = await db.Customers.AsNoTracking().OrderBy(c => c.CustomerName).ToListAsync();
    return Results.Ok(items);
});

app.MapPost("/tenant/{companyId:int}/customers", async (int companyId, CreateCustomerRequest req, ITenantDbContextFactory factory) =>
{
    await using var db = await factory.CreateDbContextAsync(companyId);

    var exists = await db.Customers.AnyAsync(c => c.CustomerCode == req.CustomerCode);
    if (exists)
        return Results.Conflict(new { message = $"CustomerCode '{req.CustomerCode}' already exists." });

    var customer = new Customer
    {
        CustomerCode = req.CustomerCode.Trim(),
        CustomerName = req.CustomerName.Trim(),
        ContactNumber = req.ContactNumber.Trim(),
        Email = req.Email.Trim(),
        Address = req.Address.Trim(),
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    db.Customers.Add(customer);
    await db.SaveChangesAsync();
    return Results.Created($"/tenant/{companyId}/customers/{customer.CustomerId}", customer);
});

app.MapGet("/tenant/{companyId:int}/suppliers", async (int companyId, ITenantDbContextFactory factory) =>
{
    await using var db = await factory.CreateDbContextAsync(companyId);
    var items = await db.Suppliers.AsNoTracking().OrderBy(s => s.SupplierName).ToListAsync();
    return Results.Ok(items);
});

app.MapPost("/tenant/{companyId:int}/suppliers", async (int companyId, CreateSupplierRequest req, ITenantDbContextFactory factory) =>
{
    await using var db = await factory.CreateDbContextAsync(companyId);

    var exists = await db.Suppliers.AnyAsync(s => s.SupplierCode == req.SupplierCode);
    if (exists)
        return Results.Conflict(new { message = $"SupplierCode '{req.SupplierCode}' already exists." });

    var supplier = new Supplier
    {
        SupplierCode = req.SupplierCode.Trim(),
        SupplierName = req.SupplierName.Trim(),
        ContactPerson = req.ContactPerson.Trim(),
        ContactNumber = req.ContactNumber.Trim(),
        Email = req.Email.Trim(),
        Address = req.Address.Trim(),
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    db.Suppliers.Add(supplier);
    await db.SaveChangesAsync();
    return Results.Created($"/tenant/{companyId}/suppliers/{supplier.SupplierId}", supplier);
});

// -------------------- AUTH ENDPOINTS --------------------

app.MapPost("/auth/login", async (
    ErpLoginRequest req,
    MasterErpDbContext masterDb,
    UserManager<ApplicationUser> userManager) =>
{
    ApplicationUser? user;
    Company? company = null;

    if (string.IsNullOrWhiteSpace(req.CompanyCode) ||
        req.CompanyCode.Trim().Equals("ADMIN", StringComparison.OrdinalIgnoreCase))
    {
        user = await masterDb.Users.FirstOrDefaultAsync(u =>
            u.UserName == req.Username.Trim() &&
            u.Role == "SuperAdmin" &&
            u.IsActive);
    }
    else
    {
        var code = req.CompanyCode.Trim();
        company = await masterDb.Companies.AsNoTracking()
            .FirstOrDefaultAsync(c => c.CompanyCode == code && c.IsActive);

        if (company is null)
            return Results.Unauthorized();

        user = await masterDb.Users.FirstOrDefaultAsync(u =>
            u.CompanyId == company.CompanyId &&
            u.UserName == req.Username.Trim() &&
            u.IsActive);
    }

    if (user is null || !await userManager.CheckPasswordAsync(user, req.Password))
        return Results.Unauthorized();

    var permissions = string.IsNullOrWhiteSpace(user.Permissions)
        ? Array.Empty<string>()
        : user.Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    return Results.Ok(new
    {
        companyId = company?.CompanyId ?? 0,
        companyCode = company?.CompanyCode ?? "ADMIN",
        companyName = company?.CompanyName ?? "Honey & Crumb Platform",
        username = user.UserName,
        fullName = user.FullName,
        role = user.Role,
        subscriptionPlan = company?.SubscriptionPlan ?? "Medium",
        permissions
    });
});

if (app.Environment.IsDevelopment())
{
    app.MapPost("/auth/seed-dev", async (MasterErpDbContext masterDb, UserManager<ApplicationUser> userManager) =>
    {
        async Task EnsureUser(string username, string password, int? companyId, string fullName, string role, string perms)
        {
            if (await userManager.FindByNameAsync(username) is not null) return;
            var u = new ApplicationUser
            {
                UserName = username,
                CompanyId = companyId,
                FullName = fullName,
                Role = role,
                Permissions = perms,
                IsActive = true
            };
            await userManager.CreateAsync(u, password);
        }

        await EnsureUser("superadmin", "Admin123!", null, "Developer Super Admin", "SuperAdmin", "");
        await EnsureUser("owner1", "Owner123!", 1, "Tenant A Owner", "Owner", "");
        await EnsureUser("manager1", "Manager123!", 1, "Tenant A Manager", "Manager", "Dashboard,Menu,Customers,Ingredients,Supply chain,Operational reports");
        await EnsureUser("staff1", "Staff123!", 1, "Tenant A Staff", "Staff", "Dashboard,Sales,Record payment,Track order status");

        return Results.Ok(new { message = "Dev users seeded." });
    });
}

app.Run();

record ErpLoginRequest(string CompanyCode, string Username, string Password);
record CreateCompanyRequest(string CompanyCode, string CompanyName);
record CreateDeviceRequest(int CompanyId, string DeviceCode, string DeviceName);
record CreateCompanyDatabaseRequest(int CompanyId, string ServerName, string DatabaseName, string CredentialKey);
record CreateProductRequest(string ProductCode, string ProductName, string Category, decimal UnitPrice);
record CreateCustomerRequest(string CustomerCode, string CustomerName, string ContactNumber, string Email, string Address);
record CreateSupplierRequest(string SupplierCode, string SupplierName, string ContactPerson, string ContactNumber, string Email, string Address);