using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CascBasic.Models;
using System.Data.Entity;

namespace CascBasic.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
                             IdentityUserLogin, ApplicationUserRole, IdentityUserClaim>
    {
        //new public virtual DbSet<ApplicationRole> Roles { get; set; }
        public virtual DbSet<ApplicationGroup> Groups { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationGroup>()
               .HasMany(p => p.Roles)
               .WithMany(r => r.Groups)
               .Map(mc =>
               {
                   mc.MapLeftKey("GroupId");
                   mc.MapRightKey("RoleId");
                   mc.ToTable("AspNetGroupRoles");
               });

            modelBuilder.Entity<ApplicationUser>()
               .HasMany(p => p.Groups)
               .WithMany(r => r.Users)
               .Map(mc =>
               {
                   mc.MapLeftKey("GroupId");
                   mc.MapRightKey("UserId");
                   mc.ToTable("AspNetUserGroups");
               });
        }
    }
}