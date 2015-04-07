namespace GossipBook.Services.Controllers
{
    using System;
    using System.Web.Http;
    using GossipBook.Data;

    public abstract class BaseController : ApiController
    {
        protected IGossipBookDbContext db;

        protected BaseController(IGossipBookDbContext db = null)
        {
            this.db = db ?? new GossipBookDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var dbAsIDisposable = this.db as IDisposable;
                if (dbAsIDisposable != null)
                {
                    dbAsIDisposable.Dispose();
                    this.db = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}
