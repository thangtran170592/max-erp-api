using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class IdentityConfig : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.ToTable("ApplicationRoles");
        }
    }

    public class UserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("ApplicationUsers");
            builder.HasKey(e => e.Id);

            builder.Property(u => u.OrderNumber)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            builder.Property(u => u.AccountStatus)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(u => u.Gender)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(u => u.Level)
                .HasConversion<int>()
                .IsRequired();
        }
    }

    public class IdentityUserClaimConfig : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
        {
            builder.ToTable("ApplicationUser_Claims");
        }
    }

    public class IdentityRoleClaimConfig : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
        {
            builder.ToTable("ApplicationRole_Claims");
        }
    }

    public class IdentityUserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
        {
            builder.ToTable("ApplicationUser_Roles");
        }
    }

    public class IdentityUserLoginConfig : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
        {
            builder.ToTable("ApplicationUser_Logins");
        }
    }

    public class IdentityUserTokenConfig : IEntityTypeConfiguration<IdentityUserToken<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
        {
            builder.ToTable("ApplicationUser_Tokens");
        }
    }
}
