namespace GossipBook.Services.Controllers
{
    using System;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using GossipBook.Models;
    using GossipBook.Services.Models;

    [Authorize]
    [EnableCors("*", "*", "*")]
    public class UsersController : BaseController
    {
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
                return this.BadRequest("A user with the provided username do not exist.");
            }

            var infoToReturn = new
            {
                Friends = user.Friends.Select(f => f.UserName),
                Groups = user.Groups.Select(g => g.Name),
                Wall = user.Wall.Posts.Select(p => new
                {
                    p.Id,
                    p.Content,
                    p.PostedAt,
                    p.User.UserName,
                    Comments = p.Comments.Select(c => new
                    {
                        c.Id,
                        c.Content,
                        c.PostedAt,
                        c.User.UserName
                    })
                })
            };

            return this.Ok(infoToReturn);
        }

        [HttpPost]
        [Route("api/Users/AddFriend/{username}")]
        public IHttpActionResult AddFriend(string username)
        {
            var user = this.Db.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return this.BadRequest("A user with the provided username do not exist.");
            }

            var currentUser = this.GetCurrentUser();
            if (user.Id == currentUser.Id)
            {
                return this.BadRequest("You cannot be friend with yourself.");
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
        [Route("api/Users/AddPost/{username}")]
        public IHttpActionResult PostOnWall(string username, PostDataModel postDataModel)
        {
            var user = this.Db.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return this.BadRequest("A user with the provided username do not exist.");
            }

            if (postDataModel == null || string.IsNullOrWhiteSpace(postDataModel.Content))
            {
                return this.BadRequest("You must provide post content.");
            }

            var currentUser = this.GetCurrentUser();
            if (currentUser.Id != user.Id &&
                (!currentUser.Friends.Contains(user) || !user.Friends.Contains(currentUser)))
            {
                return this.BadRequest("You can only post on your friends' walls.");
            }

            var newPost = new Post
            {
                Content = postDataModel.Content,
                UserId = currentUser.Id,
                WallId = user.WallId,
                PostedAt = DateTime.Now
            };

            this.Db.Posts.Add(newPost);
            this.Db.SaveChanges();

            return this.Ok("Post added successfully.");
        }
    }
}
