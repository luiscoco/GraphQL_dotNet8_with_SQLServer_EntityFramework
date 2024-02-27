# How to create .NET 8 GraphQL API with SQLServer and EntityFramework

## 1. Pull and run the SQLServer Docker image with Docker Desktop



## 2. Create a new .NET 8 WebAPI with Visual Studio 2022



## 3. Create the project structure



## 4. Add project dependencies 



## 5. Add the models

**Author.cs**

```csharp
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraphQLDemo.Models
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)] // Adjust max length as necessary
        public string Name { get; set; }

        // Virtual for enabling lazy loading
        public virtual List<Post> Posts { get; set; } = new List<Post>();
    }
}
```

**Post.cs**

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraphQLDemo.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)] // Adjust max length as necessary for Title
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }

        // Virtual for enabling lazy loading
        public virtual Author Author { get; set; }
    }
}
```

**AppDbContext.cs**

```csharp
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
```


## 6. Add the Service


## 7. Add GraphQL Types, Query and Mutations


## 8. Modify application middleware (program.cs)

```csharp
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using GraphQLDemo.GraphQL;
using GraphQLDemo.Services;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configure Entity Framework Core with SQL Server
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IPostService, PostService>();

// Add GraphQL services
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddType<AuthorType>()
    .AddType<PostType>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseRouting();

// Use GraphQL middleware
app.MapGraphQL();

app.Run();
```

## 9. Modify appsettings.json



## 10. Add database Migrations



## 11. Run and Test the application




