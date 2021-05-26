using FairyTale.Models;
using FairyTale.Web.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FairyTale.Web.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }

        [Authorize]
        public ActionResult Tales()
        {
            ViewBag.Message = "Казки";

            return View();
        }

        [Authorize]
        public ActionResult TaleView(int id)
        {
            ViewBag.Message = "Казка";
            var item = db.Tales.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                var claim = ((System.Security.Claims.ClaimsIdentity)User.Identity).Claims.FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                var status = db.UserTales.FirstOrDefault(i => i.TalesId == item.Id && i.UserId == claim.Value);
                if(status == null)
                {
                    status = new UserTale
                    {
                        UserId = claim.Value,
                        TalesId = item.Id
                    };
                    db.UserTales.Add(status);
                    db.SaveChanges();
            }
                return View(new TaleViewModel { Name = item.Name, Text = item.Text });
            }                
            return View(new TaleViewModel { Name = "Книжка не знайдена", Text = "" });
        }
    }
}