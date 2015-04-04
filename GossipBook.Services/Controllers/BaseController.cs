namespace GossipBook.Services.Controllers
{
    using System.Web.Http;
    using GossipBook.Data;

    public abstract class BaseController : ApiController
    {
        protected IGossipBookDbContext db;

        protected BaseController(IGossipBookDbContext db = null)
        {
            this.db = db ?? new GossipBookDbContext();
        }
    }
}