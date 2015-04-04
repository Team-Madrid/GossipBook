namespace GossipBook.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class User : IdentityUser
    {
        private ICollection<User> friends;
        private ICollection<Group> groups;
        private Wall wall;
        private ICollection<Post> posts;
        private ICollection<Comment> comment;

        public User()
        {
            this.friends = new HashSet<User>();
            this.groups = new HashSet<Group>();
            this.posts = new HashSet<Post>();
            this.comment = new List<Comment>();
            this.wall = new Wall();
        }

        [Required]
        public int WallId { get; set; }

        public virtual Wall Wall
        {
            get { return this.wall; }
            set { this.wall = value;  }
        }

        public virtual ICollection<User> Friends
        {
            get { return this.friends; }
            set { this.friends = value; }
        }

        public virtual ICollection<Group> Groups
        {
            get { return this.groups; }
            set { this.groups = value; }
        }

        public virtual ICollection<Post> Posts
        {
            get { return this.posts; }
            set { this.posts = value; }
        }

        public virtual ICollection<Comment> Comment
        {
            get { return this.comment; }
            set { this.comment = value; }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            return await manager.CreateIdentityAsync(this, authenticationType);
        }
    }
}
