using GraphQLDemo.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GraphQLDemo.Services
{
    public class PostService : IPostService
    {
        private readonly BlogDbContext _context;

        public PostService(BlogDbContext context)
        {
            _context = context;
        }

        public Post GetPostById(int id)
        {
            return _context.Posts.Include(p => p.Author).FirstOrDefault(p => p.Id == id);
        }

        public List<Post> GetAllPosts()
        {
            return _context.Posts.Include(p => p.Author).ToList();
        }

        public Post AddPost(Post post)
        {
            // Check if the author exists in the database
            var author = _context.Authors.FirstOrDefault(a => a.Id == post.AuthorId);
            if (author == null)
            {
                // Optionally, you can handle this case differently, 
                // e.g., throw a custom exception or handle the error in a way that's appropriate for your application
                throw new Exception($"Author with ID {post.AuthorId} not found.");
            }

            // If the author exists, proceed to add the post
            post.Author = author; // Explicitly associate the author with the post
            var newPost = _context.Posts.Add(post).Entity;
            _context.SaveChanges();

            // Explicitly load the Author to ensure it's included when the Post is returned
            // If using EF Core 5 or later, use the ChangeTracker to avoid another database round trip
            // Otherwise, you might need to manually fetch the post with the author again if it's not being tracked
            _context.Entry(newPost).Reference(p => p.Author).Load(); // Ensure this is EF Core 5.0+ for direct support

            return newPost;
        }


    }
}
