using Microsoft.EntityFrameworkCore;
using GraphQLDemo.Models;

public class BlogDbContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Post> Posts { get; set; }

    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed initial data
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, Name = "Jane Austen" },
            new Author { Id = 2, Name = "Charles Dickens" }
        );

        modelBuilder.Entity<Post>().HasData(
            new Post { Id = 1, Title = "Exploring GraphQL", Content = "GraphQL offers a more efficient way to design web APIs.", AuthorId = 1 },
            new Post { Id = 2, Title = "Advantages of GraphQL", Content = "One major advantage of GraphQL is it allows clients to request exactly what they need.", AuthorId = 1 }
        );
    }
}
