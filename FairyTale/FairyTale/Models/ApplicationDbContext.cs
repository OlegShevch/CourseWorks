using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FairyTale.Models
{

    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<Favorite> Favorites { get; set; }
        public virtual DbSet<Tale> Tales { get; set; }
        public virtual DbSet<UserTale> UserTales { get; set; }
        public virtual DbSet<TaleUserRate> TaleUserRates { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tale>()
                .HasMany(e => e.Favorites)
                .WithRequired(e => e.Tales)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tale>()
                .HasMany(e => e.UserTales)
                .WithRequired(e => e.Tales)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Tale>()
                .HasMany(e => e.TaleUserRates)
                .WithRequired(e => e.Tales)
                .WillCascadeOnDelete(false);
        }
    }
}
