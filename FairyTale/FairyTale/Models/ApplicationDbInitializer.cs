using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FairyTale.Models
{
    public class ApplicationDbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            bool itWorks = InitializeIdentityForEF(context);
            base.Seed(context);
        }

        public static bool InitializeIdentityForEF(ApplicationDbContext ctx)
        {
            DbContextTransaction transaction = null;
            bool succeeded = false;
            try
            {
                transaction = ctx.Database.BeginTransaction();
                CreateRoles(ctx);
                CreateUsers(ctx);
                ctx.SaveChanges();
                transaction.Commit();
                succeeded = true;
            }
            catch (Exception ex)
            {
                if (transaction != null) { transaction.Rollback(); transaction.Dispose(); }
                succeeded = false;
            }
            return succeeded;
        }

        private static void CreateUsers(ApplicationDbContext ctx)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));
            var user = new ApplicationUser() { UserName = "soothest@gmail.com", Email = "soothest@gmail.com", FirstName = "Олег", LastName = "Шевчук" };
            var password = "administrator";
            var role = Role.Administrator.ToString();
            var adminresult = UserManager.Create(user, password);

            if (adminresult.Succeeded)
            {
                var result = UserManager.AddToRole(user.Id, role);
            }
        }

        private static void CreateRoles(ApplicationDbContext ctx)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ctx));

            foreach (EnumItem enumItem in EnumExtensions.GetItems<Role>())
            {
                var roleName = ((Role)enumItem.Id).ToString();
                if (!roleManager.RoleExists(roleName))
                {
                    var roleresult = roleManager.Create(new IdentityRole(roleName));
                }
            }
        }
    }
}