namespace GossipBook.Services.Controllers
{
    using System;
    using System.Web.Http;
    using System.Threading;
    using GossipBook.Data;
    using GossipBook.Models;
    using Microsoft.AspNet.Identity;

    public abstract class BaseController : ApiController
    {
        protected BaseController(IGossipBookDbContext db = null)
        {
            this.Db = db ?? new GossipBookDbContext();
        }

        protected IGossipBookDbContext Db { get; private set; }

        protected string GetCurrentUserId()
        {
            return Thread.CurrentPrincipal.Identity.GetUserId();
        }

        protected User GetCurrentUser()
        {
            return this.Db.Users.Find(this.GetCurrentUserId());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var dbAsIDisposable = this.Db as IDisposable;
                if (dbAsIDisposable != null)
                {
                    dbAsIDisposable.Dispose();
                    this.Db = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
