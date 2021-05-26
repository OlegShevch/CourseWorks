using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using FairyTale.Models;
using Microsoft.AspNet.Identity.Owin;

namespace FairyTale.Web.Controllers
{
    public class TalesController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Tales
        public IQueryable<Tale> GetTales()
        {
            return db.Tales;
        }

        // GET: api/Tales/GetDetails
        public IQueryable GetDetails(string id)
        {
            var user = HttpContext.Current.GetOwinContext().Authentication.User;
            var claim = ((System.Security.Claims.ClaimsIdentity)user.Identity).Claims.FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            var items = db.Tales.ToArray().OrderBy(i => i.Rating).ToArray();
            var userTales = db.UserTales.Where(u => u.UserId == claim.Value).ToDictionary(k => k.TalesId);
            var favorites = db.Favorites.Where(u => u.UserId == claim.Value).ToDictionary(k => k.TalesId);
            return items.Select(i => new
            {
                Id = i.Id,
                Name = i.Name,
                Rating = i.Rating,
                Description = i.Description,
                CreatedOn = i.CreatedOn,
                Status = userTales.ContainsKey(i.Id),
                Favorite = favorites.ContainsKey(i.Id)
            }).AsQueryable();
        }
                

        // GET: api/Tales/5
        [ResponseType(typeof(Tale))]
        public async Task<IHttpActionResult> GetTale(int id)
        {
            Tale tale = await db.Tales.FindAsync(id);
            if (tale == null)
            {
                return NotFound();
            }

            return Ok(tale);
        }

        // PUT: api/Tales/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTale(int id, Tale tale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tale.Id)
            {
                return BadRequest();
            }

            db.Entry(tale).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        public async Task<IHttpActionResult> SetRating(string Id, [FromBody] int value)
        {
            var taleId = Convert.ToInt32(Id);
            var user = HttpContext.Current.GetOwinContext().Authentication.User;
            if (user == null)
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            var claim = ((System.Security.Claims.ClaimsIdentity)user.Identity).Claims.FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            var taleUserRate = db.TaleUserRates.FirstOrDefault(i => i.TaleId == taleId && i.UserId == claim.Value);
            if (taleUserRate != null)
            {
                return StatusCode(HttpStatusCode.NotFound);
            }

            taleUserRate = new TaleUserRate
            {
                TaleId = taleId,
                UserId = claim.Value
            };

            db.TaleUserRates.Add(taleUserRate);
            
            var tale = db.Tales.FirstOrDefault(i => i.Id == taleId);
            if(tale == null)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }
            tale.RatingCount = (tale.RatingCount ?? 0) + 1;
            tale.RatingValue = (tale.RatingValue ?? 0) + value;

            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        public async Task<IHttpActionResult> SetFavorite(string Id)
        {
            var taleId = Convert.ToInt32(Id);
            var user = HttpContext.Current.GetOwinContext().Authentication.User;
            if (user == null)
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            var claim = ((System.Security.Claims.ClaimsIdentity)user.Identity).Claims.FirstOrDefault(i => i.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            var favorite = db.Favorites.FirstOrDefault(i => i.TalesId == taleId && i.UserId == claim.Value);
            if (favorite != null)
            {
                db.Favorites.Remove(favorite);
                await db.SaveChangesAsync();
                return StatusCode(HttpStatusCode.NoContent);
            }

            favorite = new Favorite
            {
                TalesId = taleId,
                UserId = claim.Value
            };

            db.Favorites.Add(favorite);

            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }


        // POST: api/Tales
        [ResponseType(typeof(Tale))]
        public async Task<IHttpActionResult> PostTale(Tale tale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                db.Tales.Add(tale);
                await db.SaveChangesAsync();

                return CreatedAtRoute("DefaultApi", new { id = tale.Id }, tale);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // DELETE: api/Tales/
        [ResponseType(typeof(Tale))]
        public async Task<IHttpActionResult> DeleteTale(int id)
        {
            Tale tale = await db.Tales.FindAsync(id);
            if (tale == null)
            {
                return NotFound();
            }
            db.UserTales.RemoveRange(db.UserTales.Where(i => i.TalesId == id));
            db.TaleUserRates.RemoveRange(db.TaleUserRates.Where(i => i.TaleId == id));
            db.Favorites.RemoveRange(db.Favorites.Where(i => i.TalesId == id));
            db.Tales.Remove(tale);
            await db.SaveChangesAsync();

            return Ok(tale);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TaleExists(int id)
        {
            return db.Tales.Count(e => e.Id == id) > 0;
        }
    }
}