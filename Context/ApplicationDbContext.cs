using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CascBasic.Models;
using System.Data.Entity;

namespace CascBasic.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        new public virtual DbSet<ApplicationRole> Roles { get; set; }
        public virtual DbSet<Group> Groups { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Group>()
               .HasMany(p => p.Roles)
               .WithMany(r => r.Groups)
               .Map(mc =>
               {
                   mc.MapLeftKey("Group_Id");
                   mc.MapRightKey("ApplicationRole_Id");
                   mc.ToTable("GroupRoles");
               });

            modelBuilder.Entity<ApplicationUser>()
               .HasMany(p => p.Groups)
               .WithMany(r => r.Users)
               .Map(mc =>
               {
                   mc.MapLeftKey("Group_Id");
                   mc.MapRightKey("ApplicationUser_Id");
                   mc.ToTable("AspNetUserGroups");
               });
        }
        //public System.Data.Entity.DbSet<CascBasic.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}