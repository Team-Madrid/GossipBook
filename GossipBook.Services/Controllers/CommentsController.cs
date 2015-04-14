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
    public class CommentsController : BaseController
    {
        [HttpGet]
        [Route("api/Comments/{postId}")]
        public IHttpActionResult GetComments(int postId)
        {
            var post = this.Db.Posts.Find(postId);
            if (post == null)
            {
                return this.BadRequest("There is no such post.");
            }

            var commentsToReturn = this.Db.Comments.Select(c => new
            {
                c.Id,
                c.Content,
                c.PostedAt,
                c.User.UserName
            });

            return this.Ok(commentsToReturn);
        }

        [HttpPost]
        [Route("api/Comments/Add/{postId}")]
        public IHttpActionResult AddComment(int postId, CommentDataModel commentDataModel)
        {
            var post = this.Db.Posts.Find(postId);
            if (post == null)
            {
                return this.BadRequest("There is no such post.");
            }

            if (commentDataModel == null || string.IsNullOrWhiteSpace(commentDataModel.Content))
            {
                return this.BadRequest("You must provide comment content.");
            }

            var currentUser = this.GetCurrentUser();
            if (currentUser.Id != post.UserId && 
                (!currentUser.Friends.Contains(post.User) || !post.User.Friends.Contains(currentUser)))
            {
                return this.BadRequest("You can only comment on your friends' posts.");
            }

            var comment = new Comment
            {
                Content = commentDataModel.Content,
                PostedAt = DateTime.Now,
                PostId = post.Id,
                UserId = currentUser.Id
            };

            this.Db.Comments.Add(comment);
            this.Db.SaveChanges();

            return this.Ok("Comment added successfully.");
        }

        [HttpPut]
        public IHttpActionResult Edit(int id, CommentDataModel commentDataModel)
        {
            var comment = this.Db.Comments.Find(id);
            if (comment == null)
            {
                return this.BadRequest("There is no such comment.");
            }

            if (comment.UserId != this.GetCurrentUserId())
            {
                return this.BadRequest("You can edit only your own comments.");
            }

            if (commentDataModel == null || string.IsNullOrWhiteSpace(commentDataModel.Content))
            {
                return this.BadRequest("You must provide new comment content.");
            }

            comment.Content = commentDataModel.Content;
            this.Db.SaveChanges();

            return this.Ok("The comment was edited successfully.");
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            var comment = this.Db.Comments.Find(id);
            if (comment == null)
            {
                return this.BadRequest("There is no such post.");
            }

            if (comment.UserId != this.GetCurrentUserId())
            {
                return this.BadRequest("You can delete only your own comments.");
            }

            this.Db.Comments.Remove(comment);
            this.Db.SaveChanges();

            return this.Ok("The comment was deleted successfully.");
        }
    }
}
