using Microsoft.EntityFrameworkCore;
using Rentify.Core.Abstractions.Accessors;
using Rentify.Core.Entities;
using Rentify.DataAccess.SqlServer.Interfaces.Filters;
using Rentify.DataAccess.SqlServer.Interfaces.Interceptors;

namespace Rentify.DataAccess.SqlServer.Data;

public class RentifyDbContext : DbContext
{
    private readonly IEnumerable<ISaveChangesInterceptor> _saveChangeInterceptors;
    private readonly IEnumerable<IGlobalFilter> _globalFilters;
    private readonly DataScope _dataScope;

    public RentifyDbContext(
        DbContextOptions<RentifyDbContext> options, 
        IEnumerable<ISaveChangesInterceptor> saveChangesInterceptors, 
        IEnumerable<IGlobalFilter> globalFilters, 
        IDataScopeAccessor dataScopeAccessor) : base(options)
    {
        _saveChangeInterceptors = saveChangesInterceptors;
        _globalFilters = globalFilters;
        _dataScope = dataScopeAccessor.DataScope;
    }

    // DbSets for 'dbo' entities
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<MediaFileLink> MediaFileLinks { get; set; }
    public DbSet<MediaFileVariant> MediaFileVariants { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantEmergencyContact> TenantEmergencyContacts { get; set; }
    public DbSet<Unit> Units { get; set; }

    // DbSets for 'event' entities
    public DbSet<EventOutbox> EventOutboxEntries { get; set; }

    public DataScope DataScope => _dataScope;
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentifyDbContext).Assembly);

        foreach(var filter in _globalFilters)
        {
            filter.OnModelCreating(modelBuilder, this);
        }
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        foreach(var interceptor in _saveChangeInterceptors)
        {
            interceptor.OnSaveChange(this);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}