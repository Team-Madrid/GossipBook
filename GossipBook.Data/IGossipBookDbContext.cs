namespace GossipBook.Data
{
    using System.Data.Entity;
    using GossipBook.Models;

    public interface IGossipBookDbContext
    {
        IDbSet<Post> Posts { get; }

        IDbSet<Wall> Walls { get; }

        IDbSet<Group> Groups { get; }

        IDbSet<Comment> Comments { get; }

        IDbSet<User> Users { get; }

        int SaveChanges();
    }
}
