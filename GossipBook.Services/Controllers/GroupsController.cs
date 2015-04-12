using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using GossipBook.Models;
using GossipBook.Services.Models;
using Microsoft.AspNet.Identity;

namespace GossipBook.Services.Controllers
{
    [Authorize]
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
                    p.Content,
                    p.PostedAt,
                    p.User.UserName
                })
            };

            return this.Ok(infoToReturn);
        }
        
        [HttpPost]
        [Route("api/Groups")]
        public IHttpActionResult Create(GroupDataModel groupDataModel)
        {
            if (groupDataModel == null || !this.ModelState.IsValid)
            {
                return this.BadRequest("The group name is requred.");
            }

            if (this.Db.Groups.Any(g => g.Name == groupDataModel.Name))
            {
                return this.BadRequest("This name is already taken.");
            }

            var newGroup = new Group
            {
                Name = groupDataModel.Name
            };

            newGroup.Users.Add(this.GetCurrentUser());
            this.Db.Groups.Add(newGroup);
            this.Db.SaveChanges();

            return this.Ok("Successfuly created group " + newGroup.Name);
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
    }
}