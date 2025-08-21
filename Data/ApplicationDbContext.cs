using Microsoft.EntityFrameworkCore;
using irevlogix_backend.Models;

namespace irevlogix_backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientContact> ClientContacts { get; set; }
        public DbSet<MaterialType> MaterialTypes { get; set; }
        public DbSet<AssetCategory> AssetCategories { get; set; }
        public DbSet<ProcessingLot> ProcessingLots { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ShipmentItem> ShipmentItems { get; set; }
        public DbSet<ProcessingStep> ProcessingSteps { get; set; }
        public DbSet<ProcessedMaterial> ProcessedMaterials { get; set; }
        public DbSet<ProcessedMaterialSales> ProcessedMaterialSales { get; set; }
        public DbSet<ProcessedMaterialTests> ProcessedMaterialTests { get; set; }
        public DbSet<ProcessedMaterialDocuments> ProcessedMaterialDocuments { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetTrackingStatus> AssetTrackingStatuses { get; set; }
        public DbSet<ChainOfCustody> ChainOfCustodyRecords { get; set; }
        public DbSet<ShipmentStatusHistory> ShipmentStatusHistories { get; set; }
        public DbSet<ShipmentDocument> ShipmentDocuments { get; set; }
        public DbSet<AssetDocument> AssetDocuments { get; set; }
        public DbSet<ApplicationSettings> ApplicationSettings { get; set; }
        public DbSet<KnowledgeBaseArticle> KnowledgeBaseArticles { get; set; }
        public DbSet<ContractorTechnician> ContractorTechnicians { get; set; }
        public DbSet<ContractorTechnicianDocument> ContractorTechnicianDocuments { get; set; }
        public DbSet<ReverseRequest> ReverseRequests { get; set; }
        public DbSet<RecoveryRequest> RecoveryRequests { get; set; }
        public DbSet<FreightLossDamageClaim> FreightLossDamageClaims { get; set; }
        public DbSet<VendorContract> VendorContracts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Tenant>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ClientId).IsRequired();
                entity.HasIndex(e => e.ClientId).IsUnique();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired();
                entity.HasIndex(e => new { e.Email, e.ClientId }).IsUnique();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => new { e.Name, e.ClientId }).IsUnique();
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Module).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => new { e.Name, e.Module, e.Action, e.ClientId }).IsUnique();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Role)
                    .WithMany(e => e.UserRoles)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.UserId, e.RoleId, e.ClientId }).IsUnique();
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Role)
                    .WithMany(e => e.RolePermissions)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Permission)
                    .WithMany(e => e.RolePermissions)
                    .HasForeignKey(e => e.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.RoleId, e.PermissionId, e.ClientId }).IsUnique();
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => new { e.CompanyName, e.ClientId }).IsUnique();
            });


            modelBuilder.Entity<ClientContact>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.HasIndex(e => new { e.Email, e.ClientId }).IsUnique();
            });

            modelBuilder.Entity<MaterialType>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => new { e.Name, e.ClientId }).IsUnique();
            });

            modelBuilder.Entity<AssetCategory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => new { e.Name, e.ClientId }).IsUnique();
            });

            modelBuilder.Entity<ProcessingLot>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LotNumber).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => new { e.LotNumber, e.ClientId }).IsUnique();
                entity.HasOne(e => e.Operator)
                    .WithMany()
                    .HasForeignKey(e => e.OperatorUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ShipmentNumber).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => new { e.ShipmentNumber, e.ClientId }).IsUnique();
                entity.HasOne(e => e.OriginatorClient)
                    .WithMany()
                    .HasForeignKey(e => e.OriginatorClientId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.ClientContact)
                    .WithMany(cc => cc.Shipments)
                    .HasForeignKey(e => e.ClientContactId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ShipmentItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
                entity.HasOne(e => e.Shipment)
                    .WithMany(e => e.ShipmentItems)
                    .HasForeignKey(e => e.ShipmentId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.MaterialType)
                    .WithMany()
                    .HasForeignKey(e => e.MaterialTypeId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.AssetCategory)
                    .WithMany()
                    .HasForeignKey(e => e.AssetCategoryId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(e => e.ProcessingLot)
                    .WithMany(e => e.IncomingShipmentItems)
                    .HasForeignKey(e => e.ProcessingLotId)
                    .OnDelete(DeleteBehavior.SetNull);
            });


            modelBuilder.Entity<ProcessingStep>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StepName).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.ProcessingLot)
                    .WithMany(e => e.ProcessingSteps)
                    .HasForeignKey(e => e.ProcessingLotId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ResponsibleUser)
                    .WithMany()
                    .HasForeignKey(e => e.ResponsibleUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ProcessedMaterial>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(200);
                entity.HasOne(e => e.ProcessingLot)
                    .WithMany(e => e.ProcessedMaterials)
                    .HasForeignKey(e => e.ProcessingLotId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.MaterialType)
                    .WithMany(mt => mt.ProcessedMaterials)
                    .HasForeignKey(e => e.MaterialTypeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ProcessedMaterialSales>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.ProcessedMaterial)
                    .WithMany()
                    .HasForeignKey(e => e.ProcessedMaterialId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Vendor)
                    .WithMany()
                    .HasForeignKey(e => e.VendorId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.Property(e => e.Carrier).HasMaxLength(500);
                entity.Property(e => e.TrackingNumber).HasMaxLength(500);
                entity.Property(e => e.InvoiceId).HasMaxLength(500);
                entity.Property(e => e.ClientId).HasMaxLength(500);
                entity.Property(e => e.InvoiceStatus).HasMaxLength(50);
            });

            modelBuilder.Entity<ProcessedMaterialTests>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(e => e.ProcessedMaterial)
                    .WithMany()
                    .HasForeignKey(e => e.ProcessedMaterialId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.ClientId).HasDatabaseName("idx_pmt_clientid");
                entity.HasIndex(e => e.ProcessedMaterialId).HasDatabaseName("idx_pmt_material");
            });

            modelBuilder.Entity<ProcessedMaterialDocuments>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.HasOne(e => e.ProcessedMaterial)
                    .WithMany()
                    .HasForeignKey(e => e.ProcessedMaterialId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.ClientId).HasDatabaseName("idx_pmd_clientid");
                entity.HasIndex(e => e.ProcessedMaterialId).HasDatabaseName("idx_pmd_material");
            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AssetID).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => new { e.AssetID, e.ClientId }).IsUnique();
            });

            modelBuilder.Entity<AssetTrackingStatus>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StatusName).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => new { e.StatusName, e.ClientId }).IsUnique();
            });

            modelBuilder.Entity<ChainOfCustody>(entity =>
            {
                entity.ToTable("ChainOfCustody");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Asset)
                    .WithMany(e => e.ChainOfCustodyRecords)
                    .HasForeignKey(e => e.AssetId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ShipmentStatusHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Shipment)
                    .WithMany(s => s.ShipmentStatusHistories)
                    .HasForeignKey(e => e.ShipmentId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ShipmentDocument>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Shipment)
                    .WithMany(s => s.ShipmentDocuments)
                    .HasForeignKey(e => e.ShipmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<AssetDocument>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Asset)
                    .WithMany()
                    .HasForeignKey(e => e.AssetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<ApplicationSettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SettingKey).HasMaxLength(100);
                entity.Property(e => e.SettingValue).HasMaxLength(1000);
                entity.HasIndex(e => new { e.SettingKey, e.ClientId }).IsUnique();
            });

            modelBuilder.Entity<KnowledgeBaseArticle>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.HasOne(e => e.Author)
                    .WithMany()
                    .HasForeignKey(e => e.AuthorUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ContractorTechnician>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(e => new { e.FirstName, e.LastName, e.ClientId });
            });

            modelBuilder.Entity<ContractorTechnicianDocument>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
                entity.HasOne(e => e.ContractorTechnician)
                    .WithMany()
                    .HasForeignKey(e => e.ContractorTechnicianId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ReverseRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PrimaryContactFirstName).HasMaxLength(100);
                entity.Property(e => e.PrimaryContactLastName).HasMaxLength(100);
                entity.HasIndex(e => new { e.PrimaryContactFirstName, e.PrimaryContactLastName, e.ClientId });
            });

            modelBuilder.Entity<RecoveryRequest>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.PrimaryContactFirstName).HasMaxLength(100);
                entity.Property(e => e.PrimaryContactLastName).HasMaxLength(100);
                entity.HasIndex(e => new { e.PrimaryContactFirstName, e.PrimaryContactLastName, e.ClientId });
            });

            modelBuilder.Entity<FreightLossDamageClaim>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Description).HasMaxLength(200);
                entity.Property(e => e.ClaimantCompanyName).HasMaxLength(200);
                entity.Property(e => e.ClaimantCity).HasMaxLength(100);
                entity.HasIndex(e => new { e.ClaimantCompanyName, e.DateOfClaim, e.ClientId });
            });

            modelBuilder.Entity<VendorContract>()
                .HasOne(vc => vc.Vendor)
                .WithMany()
                .HasForeignKey(vc => vc.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
