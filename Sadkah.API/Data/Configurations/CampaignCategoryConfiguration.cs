using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sadkah.API.Data.Configurations
{
    public class CampaignCategoryConfiguration : IEntityTypeConfiguration<CampaignCategory>
    {
        private static readonly DateTime SeedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<CampaignCategory> builder)
        {
            builder.HasData(
                new CampaignCategory 
                { 
                    Id = new Guid("a1b2c3d4-0001-0000-0000-000000000000"), 
                    Name = "Education",            
                    CreatedAt = SeedDate, 
                    UpdatedAt = SeedDate 
                },
                new CampaignCategory 
                { 
                    Id = new Guid("a1b2c3d4-0002-0000-0000-000000000000"), 
                    Name = "Health",               
                    CreatedAt = SeedDate, 
                    UpdatedAt = SeedDate 
                },
                new CampaignCategory 
                { 
                    Id = new Guid("a1b2c3d4-0003-0000-0000-000000000000"), 
                    Name = "Environment",          
                    CreatedAt = SeedDate, 
                    UpdatedAt = SeedDate 
                },
                new CampaignCategory 
                { 
                    Id = new Guid("a1b2c3d4-0004-0000-0000-000000000000"), 
                    Name = "Animal Welfare",       
                    CreatedAt = SeedDate, 
                    UpdatedAt = SeedDate 
                },
                new CampaignCategory 
                { 
                    Id = new Guid("a1b2c3d4-0005-0000-0000-000000000000"), 
                    Name = "Community Development", 
                    CreatedAt = SeedDate, 
                    UpdatedAt = SeedDate 
                },
                new CampaignCategory 
                { 
                    Id = new Guid("a1b2c3d4-0006-0000-0000-000000000000"), 
                    Name = "Relief and Disaster Response", 
                    CreatedAt = SeedDate, 
                    UpdatedAt = SeedDate 
                },
                new CampaignCategory 
                { 
                    Id = new Guid("a1b2c3d4-0007-0000-0000-000000000000"), 
                    Name = "Religious and Spiritual", 
                    CreatedAt = SeedDate, 
                    UpdatedAt = SeedDate 
                },
                new CampaignCategory 
                { 
                    Id = new Guid("a1b2c3d4-0008-0000-0000-000000000000"), 
                    Name = "Other", 
                    CreatedAt = SeedDate, 
                    UpdatedAt = SeedDate 
                }
            );
        }
    }
}
