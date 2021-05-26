using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace FairyTale.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }

        public ApplicationUserInRole Convert(Role roleID)
        {
            var serializedParent = JsonConvert.SerializeObject(this);
            var userInRole = JsonConvert.DeserializeObject<ApplicationUserInRole>(serializedParent);
            userInRole.RoleID = (int)roleID;
            return userInRole;
        }
    }

    public class ApplicationUserInRole : ApplicationUser
    {
        public int RoleID { get; set; }

        public string RoleName
        {
            get { return EnumExtensions.GetItem<Role>(RoleID); }
        }
    }
}