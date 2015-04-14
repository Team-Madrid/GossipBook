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
    public class GroupsController : BaseController
    {
        [HttpGet]
        [Route("api/Groups")]
        public IHttpActionResult GetAll()
        {
            var groups = this.Db.Groups
                .Select(g => g.Name)
                .ToList();

            return this.Ok(groups);
        }
        
        [HttpGet]
        [Route("api/Groups/{name}")]
        public IHttpActionResult GetGroupInfo(string name)
        {
            var group = this.Db.Groups.FirstOrDefault(g => g.Name == name);
            if (group == null)
            {
                return this.BadRequest("There is no such group.");
            }

            var infoToReturn = new
            {
                Members = group.Users.Select(u => u.UserName),
                Wall = group.Wall.Posts.Select(p => new
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
        public IHttpActionResult Create(GroupDataModel groupDataModel)
        {
            if (groupDataModel == null || !this.ModelState.IsValid || string.IsNullOrWhiteSpace(groupDataModel.Name))
            {
                return this.BadRequest("The group name is requred.");
            }

            if (this.Db.Groups.Any(g => g.Name == groupDataModel.Name))
            {
                return this.BadRequest("This name is already taken.");
            }

            var newGroup = new Group
            {
                Name = groupDataModel.Name,
                Wall = new Wall()
            };

            newGroup.Users.Add(this.GetCurrentUser());
            this.Db.Groups.Add(newGroup);
            this.Db.SaveChanges();

            return this.Ok("Successfuly created group " + newGroup.Name);
        }

        [HttpPut]
        public IHttpActionResult Rename(int id, GroupDataModel groupDataModel)
        {
            var group = this.Db.Groups.Find(id);
            if (group == null)
            {
                return this.BadRequest("There is no such group.");
            }

            if (groupDataModel == null || !this.ModelState.IsValid || string.IsNullOrWhiteSpace(groupDataModel.Name))
            {
                return this.BadRequest("The group name is requred.");
            }

            if (this.Db.Groups.Any(g => g.Name == groupDataModel.Name))
            {
                return this.BadRequest("This name is already taken.");
            }

            group.Name = groupDataModel.Name;
            this.Db.SaveChanges();

            return this.Ok("Successfuly renamed group " + group.Name);
        }


        [HttpPost]
        [Route("api/Groups/Join/{name}")]
        public IHttpActionResult Join(string name)
        {
            var group = this.Db.Groups.FirstOrDefault(g => g.Name == name);
            if (group == null)
            {
                return this.BadRequest("There is no such group.");
            }

            var currentUser = this.GetCurrentUser();
            if (group.Users.Contains(currentUser))
            {
                return this.Ok("You already are in this group.");
            }

            group.Users.Add(currentUser);
            this.Db.SaveChanges();

            return this.Ok("Successfully joined group " + group.Name);
        }

        [HttpPost]
        [Route("api/Groups/Leave/{name}")]
        public IHttpActionResult Leave(string name)
        {
            var group = this.Db.Groups.FirstOrDefault(g => g.Name == name);
            if (group == null)
            {
                return this.BadRequest("There is no such group.");
            }

            var currentUser = this.GetCurrentUser();
            if (!group.Users.Contains(currentUser))
            {
                return this.BadRequest("You already are not a member of this group.");
            }

            group.Users.Remove(currentUser);
            this.Db.SaveChanges();

            return this.Ok("Successfully left group " + group.Name);
        }

        [HttpPost]
        [Route("api/Groups/AddPost/{name}")]
        public IHttpActionResult PostOnWall(string name, PostDataModel postDataModel)
        {
            var group = this.Db.Groups.FirstOrDefault(g => g.Name == name);
            if (group == null)
            {
                return this.BadRequest("There is no such group.");
            }

            if (postDataModel == null || string.IsNullOrWhiteSpace(postDataModel.Content))
            {
                return this.BadRequest("You must provide post content.");
            }

            var currentUser = this.GetCurrentUser();
            if (!group.Users.Contains(currentUser))
            {
                return this.BadRequest("You must join the group first.");
            }

            var newPost = new Post
            {
                Content = postDataModel.Content,
                UserId = currentUser.Id,
                WallId = group.WallId,
                PostedAt = DateTime.Now
            };

            this.Db.Posts.Add(newPost);
            this.Db.SaveChanges();

            return this.Ok("Post added successfully.");
        }
    }
}
