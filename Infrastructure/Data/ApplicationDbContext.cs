using Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<Broadcast> Broadcasts => Set<Broadcast>();
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<ConversationMember> ConversationMembers => Set<ConversationMember>();
        public DbSet<MessageReceipt> MessageReceipts => Set<MessageReceipt>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();
        public DbSet<WarehouseHistory> WarehouseHistories => Set<WarehouseHistory>();
        public DbSet<BroadcastRecipient> BroadcastRecipients => Set<BroadcastRecipient>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductHistory> ProductHistories => Set<ProductHistory>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<Package> Packages => Set<Package>();
        public DbSet<UnitOfMeasure> UnitOfMeasures => Set<UnitOfMeasure>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<PackageUnit> PackageUnits => Set<PackageUnit>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Position> Positions => Set<Position>();
        public DbSet<ApprovalConfig> ApprovalConfigs { get; set; }
        public DbSet<ApprovalFeature> ApprovalFeatures { get; set; }
        public DbSet<ApprovalStep> ApprovalSteps { get; set; }
        public DbSet<ApprovalDocument> ApprovalDocuments { get; set; }
        public DbSet<ApprovalHistory> ApprovalHistories { get; set; }
        public DbSet<Barcode> Barcodes => Set<Barcode>();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellation = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
            return await base.SaveChangesAsync(cancellation);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}