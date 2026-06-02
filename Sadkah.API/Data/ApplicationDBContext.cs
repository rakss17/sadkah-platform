using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Sadkah.API.Data
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {
        }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Campaign>()
                .HasOne(c => c.Owner)
                .WithMany(u => u.Campaigns)
                .HasForeignKey(c => c.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Donor)
                .WithMany(u => u.Donations)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Donation>()
                .HasOne(d => d.Campaign)
                .WithMany(c => c.Donations)
                .HasForeignKey(d => d.CampaignId)
                .OnDelete(DeleteBehavior.Restrict);

            List<IdentityRole> roles = new List<IdentityRole>
            {
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
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);

        }

    }
}
