namespace GossipBook.Services.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Cors;
    using EntityFramework.Extensions;
    using GossipBook.Models;
    using GossipBook.Services.Models;

    [Authorize]
    [EnableCors("*", "*", "*")]
    public class PostsController : BaseController
    {
        [HttpGet]
        [Route("api/Posts/Wall/{id}")]
        public IHttpActionResult GetWallPosts(int id)
        {
            var wall = this.Db.Walls.Find(id);
            if (wall == null)
            {
                return this.BadRequest("There is no such wall.");
            }

            var postsToReturn = GetWallPosts(wall);

            return this.Ok(postsToReturn);
        }

        [HttpGet]
        [Route("api/Posts/User/{username}")]
        public IHttpActionResult GetUserWallPosts(string username)
        {
            var user = this.Db.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return this.BadRequest("There is no such user.");
            }

            var postsToReturn = GetWallPosts(user.Wall);

            return this.Ok(postsToReturn);
        }

        [HttpGet]
        [Route("api/Posts/Group/{name}")]
        public IHttpActionResult GetGroupWallPosts(string name)
        {
            var group = this.Db.Groups.FirstOrDefault(g => g.Name == name);
            if (group == null)
            {
                return this.BadRequest("There is no such user.");
            }

            var postsToReturn = GetWallPosts(group.Wall);

            return this.Ok(postsToReturn);
        }

        [HttpPut]
        public IHttpActionResult Edit(int id, PostDataModel postDataModel)
        {
            var post = this.Db.Posts.Find(id);
            if (post == null)
            {
                return this.BadRequest("There is no such post.");
            }

            if (post.UserId != this.GetCurrentUserId())
            {
                return this.BadRequest("You can edit only your own posts.");
            }

            if (postDataModel == null || string.IsNullOrWhiteSpace(postDataModel.Content))
            {
                return this.BadRequest("You must provide new post content.");
            }

            post.Content = postDataModel.Content;
            this.Db.SaveChanges();

            return this.Ok("The post was edited successfully.");
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var post = this.Db.Posts.Find(id);
            if (post == null)
            {
                return this.BadRequest("There is no such post.");
            }

            if (post.UserId != this.GetCurrentUserId())
            {
                return this.BadRequest("You can delete only your own posts.");
            }

            this.Db.Comments.Where(c => c.PostId == post.Id).Delete();
            this.Db.Posts.Remove(post);
            this.Db.SaveChanges();

            return this.Ok("The post was deleted successfully.");
        }

        private static IEnumerable<object> GetWallPosts(Wall wall)
        {
            return wall.Posts.Select(p => new
            {
                p.Id,
                p.Content,
                p.PostedAt,
                p.User.UserName
            });
        }
    }
}
