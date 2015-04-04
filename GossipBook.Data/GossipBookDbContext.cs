namespace GossipBook.Data
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using GossipBook.Data.Migrations;
    using GossipBook.Models;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class GossipBookDbContext : IdentityDbContext<User>, IGossipBookDbContext
    {
        public GossipBookDbContext()
            : base("GossipBookConnection", false)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<GossipBookDbContext, Configuration>());
        }

        public IDbSet<Post> Posts { get; set; }

        public IDbSet<Wall> Walls { get; set; }

        public IDbSet<Group> Groups { get; set; }

        public IDbSet<Comment> Comments { get; set; }

        public static GossipBookDbContext Create()
        {
            return new GossipBookDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasMany(u => u.Friends).WithMany();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            base.OnModelCreating(modelBuilder);
        }
    }
}
