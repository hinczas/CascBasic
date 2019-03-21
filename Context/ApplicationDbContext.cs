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
        public virtual DbSet<MenuItem> MenuItems { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<Institution> Institutions { get; set; }


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

            modelBuilder.Entity<MenuItem>()
                .HasOptional(c => c.Parent)
                .WithMany()
                .HasForeignKey(c => c.ParentId);

            modelBuilder.Entity<ApplicationGroup>()
                .HasMany(p => p.MenuItems)
                .WithMany(r => r.Groups)
                .Map(mc =>
                {
                    mc.MapLeftKey("GroupId");
                    mc.MapRightKey("MenuId");
                    mc.ToTable("GroupMenus");
                });

            modelBuilder.Entity<ApplicationGroup>()
                .HasMany(p => p.Permissions)
                .WithMany(r => r.Groups)
                .Map(mc =>
                {
                    mc.MapLeftKey("GroupId");
                    mc.MapRightKey("PermId");
                    mc.ToTable("GroupPermissions");
                });

            modelBuilder.Entity<ApplicationRole>()
                .HasMany(p => p.Permissions)
                .WithMany(r => r.Roles)
                .Map(mc =>
                {
                    mc.MapLeftKey("RoleId");
                    mc.MapRightKey("PermId");
                    mc.ToTable("RolePermissions");
                });
        }
    }
}