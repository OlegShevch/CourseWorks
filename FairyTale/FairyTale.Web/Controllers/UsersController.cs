using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Web.Http.Description;
using FairyTale.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Mvc;
using FairyTale.Web.Models;

namespace FairyTale.Web.Controllers
{
    public class UsersController : ApiController
    {
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        // GET: api/Users
        public IEnumerable<ApplicationUserInRole> GetUsers()
        {
            return UserManager.Users.ToArray().Select(u => u.Convert(EnumExtensions.Parse<Role>(UserManager.GetRoles(u.Id).First())));
        }

        // GET: api/Users/5
        [ResponseType(typeof(ApplicationUserInRole))]
        public async Task<IHttpActionResult> GetUser(string id)
        {
            ApplicationUser user = UserManager.FindById(id);
            
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.Convert(EnumExtensions.Parse<Role>(UserManager.GetRoles(user.Id).First())));
        }

        // GET: api/GetRoles
        public IQueryable<EnumItem> GetUserRoles()
        {
            return EnumExtensions.GetItems<Role>().AsQueryable();
        }

        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(string id, ApplicationUserInRole user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            ApplicationUser u = UserManager.FindById(id);
            u.UserName = user.Email;
            u.Email = user.Email;
            u.FirstName = user.FirstName;
            u.LastName = user.LastName;
            UserManager.Update(u);

            var role = ((Role) user.RoleID).ToString();
            foreach (var r in UserManager.GetRoles(id).Where(r => r != role))
            {
                UserManager.RemoveFromRole(u.Id, r);
            }
            
            var result = UserManager.AddToRole(user.Id, role);

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        [ResponseType(typeof(ApplicationUserInRole))]
        public async Task<IHttpActionResult> PostUser(ApplicationUserInRole user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.Id = Guid.NewGuid().ToString();
            user.UserName = user.Email;
            var adminresult = UserManager.Create(user, "password");

            if (!adminresult.Succeeded)
                return InternalServerError();

            var role = ((Role)user.RoleID).ToString();
            var result = UserManager.AddToRole(user.Id, role);

            try
            {
                var ctrl = new AccountController(HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>(), HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>());

                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //var callbackUrl = ctrl.Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: ctrl.Request.Url.Scheme);
                var callbackUrl = this.Url.Link("Default", new { Controller = "Account", Action = "ResetPassword", param1 = new { userId = user.Id, code = code } });

                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
            }
            catch (Exception e)
            {
                throw;
            }

            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(ApplicationUserInRole))]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            ApplicationUser user = UserManager.FindById(id);
            if (user == null)
            {
                return NotFound();
            }

            UserManager.Delete(user);
            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    db.Dispose();
            //}
            //base.Dispose(disposing);
        }

        private bool UserExists(string id)
        {
            return UserManager.Users.Any(e => e.Id == id);
        }
    }
}