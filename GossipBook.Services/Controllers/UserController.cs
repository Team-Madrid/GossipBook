namespace GossipBook.Services.Controllers
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.Owin;
    using Microsoft.Owin.Security;

    using GossipBook.Models;
    using GossipBook.Services.Models;

    [Authorize]
    public class UserController : BaseController
    {
        private ApplicationUserManager userManager;

        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            this.UserManager = userManager;
            this.AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return this.userManager ?? this.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                this.userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        [HttpGet]
        [Route("api/Users")]
        public IHttpActionResult GetAll()
        {
            var users = this.Db.Users
                .Select(u => u.UserName)
                .ToList();

            return this.Ok(users);
        }

        [HttpGet]
        [Route("api/Users/{username}")]
        public IHttpActionResult GetUserInfo(string username)
        {
            var user = this.Db.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return this.BadRequest("A user with the provided id do not exist.");
            }

            var infoToReturn = new
            {
                Friends = user.Friends.Select(f => f.UserName),
                Groups = user.Groups.Select(g => g.Name),
                Wall = user.Wall.Posts.Select(p => new
                {
                    p.Content,
                    p.PostedAt,
                    p.User.UserName
                })
            };

            return this.Ok(infoToReturn);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/Register")]
        public async Task<IHttpActionResult> Register(RegisterDataModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var user = new User { UserName = model.Username, Email = model.Email };

            var result = await this.UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return this.GetErrorResult(result);
            }

            return this.Ok("Registration successful.");
        }

        [HttpPost]
        [Route("api/AddFriend/{username}")]
        public IHttpActionResult AddFriend(string username)
        {
            var user = this.Db.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return this.BadRequest("A user with the provided id do not exist.");
            }

            var currentUserId = this.User.Identity.GetUserId();
            if (user.Id == currentUserId)
            {
                return this.BadRequest("You cannot be friend with yourself.");
            }

            var currentUser = this.Db.Users.Find(currentUserId);
            if (currentUser == null)
            {
                return this.Unauthorized();
            }

            if (currentUser.Friends.Contains(user))
            {
                return this.Ok("This user is already your friend.");
            }

            currentUser.Friends.Add(user);
            this.Db.SaveChanges();

            return this.Ok("Friend successfully added.");
        }

        [HttpPost]
        [Route("api/ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordDataModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var result = await this.UserManager.ChangePasswordAsync(this.User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                return this.GetErrorResult(result);
            }

            return this.Ok("Password changed successfully.");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.userManager != null)
            {
                this.userManager.Dispose();
                this.userManager = null;
            }

            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return this.InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        this.ModelState.AddModelError("", error);
                    }
                }

                if (this.ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return this.BadRequest();
                }

                return this.BadRequest(this.ModelState);
            }

            return null;
        }
    }
}
