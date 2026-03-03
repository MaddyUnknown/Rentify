using Microsoft.EntityFrameworkCore;
using Rentify.Core.Abstractions.Entities;
using Rentify.Core.Entities;
using Rentify.DataAccess.SqlServer.Interfaces.Interceptors;

namespace Rentify.DataAccess.SqlServer.Data;

public class RentifyDbContext : DbContext
{
    private readonly IEnumerable<ISaveChangesInterceptor> _saveChangeInterceptor;

    public RentifyDbContext(DbContextOptions<RentifyDbContext> options, IEnumerable<ISaveChangesInterceptor> saveChangesInterceptors) : base(options)
    {
        _saveChangeInterceptor = saveChangesInterceptors;
    }

    // DbSets for 'dbo' entities
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<MediaFileLink> MediaFileLinks { get; set; }
    public DbSet<MediaFileLinkTag> MediaFileLinkTags { get; set; }
    public DbSet<MediaFileVariant> MediaFileVariants { get; set; }
    public DbSet<Owner> Owners { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantEmergencyContact> TenantEmergencyContacts { get; set; }
    public DbSet<Unit> Units { get; set; }

    // DbSets for 'event' entities
    public DbSet<EventOutbox> EventOutboxEntries { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RentifyDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        foreach(var interceptor in _saveChangeInterceptor)
        {
            interceptor.OnSaveChange(this);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}