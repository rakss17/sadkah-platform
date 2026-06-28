using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sadkah.API.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Id = "ROLE_ADMIN",
                    Name = UserRole.Admin.ToString(),
                    NormalizedName = UserRole.Admin.ToString().ToUpper(),
                    ConcurrencyStamp = "ROLE_ADMIN"
                },
                new IdentityRole
                {
                    Id = "ROLE_USER",
                    Name = UserRole.User.ToString(),
                    NormalizedName = UserRole.User.ToString().ToUpper(),
                    ConcurrencyStamp = "ROLE_USER"
                }
            );
        }
    }
}
