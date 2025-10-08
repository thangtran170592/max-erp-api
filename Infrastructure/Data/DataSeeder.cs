using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, UserManager<ApplicationUser>? userManager = null, RoleManager<ApplicationRole>? roleManager = null)
    {
        // Seed SuperAdmin user first (if managers provided)
        if (userManager != null && roleManager != null)
        {
            await SeedSuperAdminAsync(userManager, roleManager);
        }
        // Departments
        if (!await db.Departments.AnyAsync())
        {
            var departments = new List<Department>
            {
                new Department { Id = Guid.NewGuid(), Uid = "PKHO", Name = "Phòng Kho", Description = "Kho tổng", Status = true, CreatedAt = DateTime.UtcNow },
                new Department { Id = Guid.NewGuid(), Uid = "PKHODA", Name = "Phòng Kho DA", Description = "Kho dự án A", Status = true, CreatedAt = DateTime.UtcNow },
                new Department { Id = Guid.NewGuid(), Uid = "PKHOBB", Name = "Phòng Kho BB", Description = "Kho miền Bắc", Status = true, CreatedAt = DateTime.UtcNow }
            };
            await db.Departments.AddRangeAsync(departments);
        }

        // Positions
        if (!await db.Positions.AnyAsync())
        {
            var positions = new List<Position>
            {
                new Position { Id = Guid.NewGuid(), Uid = "TPKHO", Name = "Trưởng phòng Kho", Description = "Quản lý phòng kho", Status = true, CreatedAt = DateTime.UtcNow },
                new Position { Id = Guid.NewGuid(), Uid = "KHONV", Name = "Nhân viên Kho", Description = "Nhân viên kho", Status = true, CreatedAt = DateTime.UtcNow },
                new Position { Id = Guid.NewGuid(), Uid = "CEO", Name = "Chief Executive Officer", Description = "Giám đốc điều hành", Status = true, CreatedAt = DateTime.UtcNow }
            };
            await db.Positions.AddRangeAsync(positions);
        }

        // Warehouses
        if (!await db.Warehouses.AnyAsync())
        {
            var warehouses = new List<Warehouse>
            {
                new Warehouse { Id = Guid.NewGuid(), Uid = "WHMAIN", Name = "Kho Trung Tâm", Status = true, ApprovalStatus = ApprovalStatus.Approved, CreatedAt = DateTime.UtcNow },
                new Warehouse { Id = Guid.NewGuid(), Uid = "WHNORTH", Name = "Kho Miền Bắc", Status = true, ApprovalStatus = ApprovalStatus.Approved, CreatedAt = DateTime.UtcNow },
                new Warehouse { Id = Guid.NewGuid(), Uid = "WHSOUTH", Name = "Kho Miền Nam", Status = true, ApprovalStatus = ApprovalStatus.Approved, CreatedAt = DateTime.UtcNow }
            };
            await db.Warehouses.AddRangeAsync(warehouses);
        }

        // UnitOfMeasures
        if (!await db.UnitOfMeasures.AnyAsync())
        {
            var uoms = new List<UnitOfMeasure>
            {
                new UnitOfMeasure { Id = Guid.NewGuid(), Uid = "PCS", Name = "Piece", Status = true, CreatedAt = DateTime.UtcNow },
                new UnitOfMeasure { Id = Guid.NewGuid(), Uid = "BOX", Name = "Box", Status = true, CreatedAt = DateTime.UtcNow },
                new UnitOfMeasure { Id = Guid.NewGuid(), Uid = "KG", Name = "Kilogram", Status = true, CreatedAt = DateTime.UtcNow }
            };
            await db.UnitOfMeasures.AddRangeAsync(uoms);
        }

        // ProductCategories
        if (!await db.ProductCategories.AnyAsync())
        {
            var categories = new List<ProductCategory>
            {
                new ProductCategory { Id = Guid.NewGuid(), Uid = "CATBOM", Name = "Danh mục BOM", Type = ProductCategoryType.Bom, Status = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { Id = Guid.NewGuid(), Uid = "CATMM", Name = "Danh mục MinMax", Type = ProductCategoryType.MinMax, Status = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { Id = Guid.NewGuid(), Uid = "CATOTHER", Name = "Danh mục Khác", Type = ProductCategoryType.Unknown, Status = true, CreatedAt = DateTime.UtcNow }
            };
            await db.ProductCategories.AddRangeAsync(categories);
        }

        // Packages
        if (!await db.Packages.AnyAsync())
        {
            var packages = new List<Package>
            {
                new Package { Id = Guid.NewGuid(), Uid = "PKSTD", Name = "Gói Chuẩn", Status = true, CreatedAt = DateTime.UtcNow },
                new Package { Id = Guid.NewGuid(), Uid = "PKPREM", Name = "Gói Cao Cấp", Status = true, CreatedAt = DateTime.UtcNow }
            };
            await db.Packages.AddRangeAsync(packages);
        }

        // PackageUnits (ví dụ nhiều cấp độ đóng gói)
        if (!await db.PackageUnits.AnyAsync())
        {
            var firstPackage = await db.Packages.FirstOrDefaultAsync();
            if (firstPackage != null)
            {
                var pcs = await db.UnitOfMeasures.FirstOrDefaultAsync(u => u.Uid == "PCS");
                var box = await db.UnitOfMeasures.FirstOrDefaultAsync(u => u.Uid == "BOX");
                var units = new List<PackageUnit>();
                if (pcs != null)
                {
                    units.Add(new PackageUnit { Id = Guid.NewGuid(), PackageId = firstPackage.Id, UnitId = pcs.Id, Level = 1, Quantity = 1 });
                }
                if (box != null)
                {
                    units.Add(new PackageUnit { Id = Guid.NewGuid(), PackageId = firstPackage.Id, UnitId = box.Id, Level = 2, Quantity = 10 });
                }
                if (units.Any())
                {
                    await db.PackageUnits.AddRangeAsync(units);
                }
            }
        }

        await db.SaveChangesAsync();
    }

    private static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        const string superAdminUserName = "superadmin";
        const string superAdminEmail = "superadmin@local.test";
        const string defaultPassword = "Code@123"; // Consider moving to configuration / secret store
        const string preferredRole = "Admin"; // Adjust if your highest role differs (e.g., SuperAdmin)

        var existing = await userManager.FindByNameAsync(superAdminUserName);
        if (existing != null) return; // already seeded

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = superAdminUserName,
            NormalizedUserName = superAdminUserName.ToUpperInvariant(),
            Email = superAdminEmail,
            NormalizedEmail = superAdminEmail.ToUpperInvariant(),
            EmailConfirmed = true,
            Uid = "SUPERADMIN",
            FullName = "System Super Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            JoinDate = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, defaultPassword);
        if (!createResult.Succeeded)
        {
            // You might want to log errors; for now just return
            return;
        }

        if (await roleManager.RoleExistsAsync(preferredRole))
        {
            await userManager.AddToRoleAsync(user, preferredRole);
        }
    }
}
