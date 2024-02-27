using GraphQLDemo.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GraphQLDemo.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly BlogDbContext _context;

        public AuthorService(BlogDbContext context)
        {
            _context = context;
        }

        public Author GetAuthorById(int id)
        {
            return _context.Authors.Include(a => a.Posts).FirstOrDefault(a => a.Id == id);
        }

        public List<Author> GetAllAuthors()
        {
            return _context.Authors.Include(a => a.Posts).ToList();
        }
    }
}
