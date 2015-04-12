namespace GossipBook.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Group
    {
        private ICollection<User> users;
        private Wall wall;

        public Group()
        {
            this.users = new HashSet<User>();
            this.wall = new Wall();
        }

        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [Required]
        public int WallId { get; set; }

        public virtual Wall Wall
        {
            get { return this.wall; }
            set { this.wall = value; }
        }

        public virtual ICollection<User> Users
        {
            get { return this.users; }
            set { this.users = value; }
        }
    }
}
