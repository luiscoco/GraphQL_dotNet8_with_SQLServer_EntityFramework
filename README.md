# How to create .NET 8 GraphQL API with SQLServer and EntityFramework

The source code for this example is available in this github repo: 

https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework

## 1. Pull and run the SQLServer Docker image with Docker Desktop

We run Docker Desktop application

![image](https://github.com/luiscoco/Identity_dotNET8_Authentication/assets/32194879/be7cb784-9f18-4f5f-a557-cfcc47cc3d6f)

We open a command prompt window and run the following command

```
docker run ^
  -e "ACCEPT_EULA=Y" ^
  -e "MSSQL_SA_PASSWORD=Luiscoco123456" ^
  -p 1433:1433 ^
  -d mcr.microsoft.com/mssql/server:2022-latest
```

![image](https://github.com/luiscoco/Identity_dotNET8_Authentication/assets/32194879/5661f705-44eb-4537-855e-ed04279d002e)

We can also see the image in Docker Desktop

![image](https://github.com/luiscoco/Identity_dotNET8_Authentication/assets/32194879/399896b3-0c2b-4ec0-acfc-b83f10691e5d)

And also we can see the running container in Docker Desktop

![image](https://github.com/luiscoco/Identity_dotNET8_Authentication/assets/32194879/9a1b0c37-5bc0-4129-987c-60f77db91448)

We run SSMS SQL Server Management Studio and we connect to the SQL Server running docker container

![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/5381d14a-1019-421e-903e-cf37ccb0300d)

We create the database for this sample

![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/4c0600ef-077d-49eb-a970-c733ac663e24)

## 2. Create a new .NET 8 WebAPI with Visual Studio 2022

We run Visual Studio 2022 Community Edition and we create a new project
G
![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/346b3777-7f19-439c-bb06-a24ad1503729)

We select the **api** project template

![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/79fa8f9e-b440-4b3f-bd26-78469730622f)

We input the project name and location 

![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/95ec4d51-f35e-4e56-9833-d9d042320df5)

We select the project default features

![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/8fc0435d-c623-44d9-9173-ec2bd1cdf77b)

After opening the project in Visual Studio we can delete the Controller folder and the Weatherforecast controller and service

## 3. Create the project structure

First we create the **Models** folder and inside the files: Author.cs, Post.cs and AppDbContext.cs

Then we create the **Services** folder and we place inside the files: IAuthorService.cs, AuthorService.cs, IPostService.cs and PostService.cs

We finally create the **GraphQL** folder and the **Types** subfolder. We also create the files: AuthorType.cs, PostType.cs, Query.cs and Mutation.cs 

![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/b99ba4d6-50fa-48a5-a1dc-c2b8b62452a0)

## 4. Add project dependencies 

![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/0139f24c-df09-481f-911f-dc4986c50411)

See the csproj file

```
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="HotChocolate.AspNetCore" Version="13.9.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.2" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>

</Project>
```

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

**IAuthorService.cs**

```csharp
using GraphQLDemo.Models;

namespace GraphQLDemo.Services
{
    public interface IAuthorService
    {
        Author GetAuthorById(int id);
        List<Author> GetAllAuthors();
    }
}
```

**IPostService.cs**

```
using GraphQLDemo.Models;

namespace GraphQLDemo.Services
{
    public interface IPostService
    {
        Post GetPostById(int id);
        List<Post> GetAllPosts();
        Post AddPost(Post post);
    }
}
```

**AuthorService.cs**

```csharp
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
```

**PostService.cs**

```csharp
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
```

## 7. Add GraphQL Types, Query and Mutations

**AuthorType.cs**

```csharp
using GraphQLDemo.Models;
using HotChocolate.Types;

namespace GraphQLDemo.GraphQL
{
    public class AuthorType : ObjectType<Author>
    {
        protected override void Configure(IObjectTypeDescriptor<Author> descriptor)
        {
            descriptor.Field(a => a.Id).Type<NonNullType<IntType>>();
            descriptor.Field(a => a.Name).Type<NonNullType<StringType>>();
            descriptor.Field(a => a.Posts).Type<NonNullType<ListType<NonNullType<PostType>>>>();
        }
    }
}
```

**PostType.cs**

```csharp
using GraphQLDemo.Models;
using HotChocolate.Types;

namespace GraphQLDemo.GraphQL
{
    public class PostType : ObjectType<Post>
    {
        protected override void Configure(IObjectTypeDescriptor<Post> descriptor)
        {
            descriptor.Field(p => p.Id).Type<NonNullType<IntType>>();
            descriptor.Field(p => p.Title).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.Content).Type<NonNullType<StringType>>();
            descriptor.Field(p => p.Author).Type<NonNullType<AuthorType>>();
        }
    }
}
```

**Query.cs**

```csharp
using GraphQLDemo.Models;
using HotChocolate;
using GraphQLDemo.Services;

namespace GraphQLDemo.GraphQL
{
    public class Query
    {
        public Author GetAuthor([Service] IAuthorService authorService, int id) =>
            authorService.GetAuthorById(id);

        public Post GetPost([Service] IPostService postService, int id) =>
            postService.GetPostById(id);
    }
}
```

**Mutation.cs**

```csharp
using GraphQLDemo.Models;
using HotChocolate;
using GraphQLDemo.Services;

namespace GraphQLDemo.GraphQL
{
    public class Mutation
    {
        public Post AddPost([Service] IPostService postService, CreatePostInput input) =>
            postService.AddPost(new Post
            {
                Title = input.Title,
                Content = input.Content,
                AuthorId = input.AuthorId
            });
    }

    public record CreatePostInput(string Title, string Content, int AuthorId);
}
```

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

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=GraphQLProductDB;User ID=sa;Password=Luiscoco123456;Encrypt=false;TrustServerCertificate=true;"
  },
  "AllowedHosts": "*"
}
```

## 10. Add database Migrations

We first run the initial migration

```
dotnet ef migrations add InitialSeed
```

and then we run this command to update the database

```
dotnet ef database update
```

We can verify in SSMS the new tables created 

![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/8cdf1405-6a68-4157-b625-94dcdef3d6aa)

## 11. Run and Test the application

We build and run the application in Visual Studio 

We navigate to the Banana endpoint: https://localhost:7106/graphql/

![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/53caeb3b-a90b-4634-9696-2e5ee91f7d69)

We press on the **Create document** button 

We input the samples code, see below

![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/adf8f371-15ff-4740-9cd3-6e1dda27aad8)

Query Examples

**Fetch an Author by ID**

This query retrieves an author by their id, including all posts associated with the author

```
query GetAuthor {
  author(id: 1) {
    id
    name
    posts {
      id
      title
      content
    }
  }
}
```

Replace 1 with the actual ID of the author you want to test

![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/982fabc0-9c1d-4c2d-94ce-42f33c7a94b1)

**Fetch a Post by ID**

This query retrieves a post by its id, including the author details

```
query GetPost {
  post(id: 1) {
    id
    title
    content
    author {
      id
      name
    }
  }
}
```

Replace 1 with the actual ID of the post you want to test

![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/4a80128e-bd2a-4cf8-9b4f-1c050c26aa89)

**Mutation Example (Add a New Post)**

This mutation adds a new post to the database. You need to provide a title, content, and the author's ID

```
mutation AddNewPost {
  addPost(input: {title: "New GraphQL Post", content: "Exploring GraphQL mutations.", authorId: 1}) {
    id
    title
    content
    author {
      id
      name
    }
  }
}
```

Replace "New GraphQL Post", "Exploring GraphQL mutations.", and 1 with your desired post title, content, and author ID, respectively

![image](https://github.com/luiscoco/GraphQL_dotNet8_with_SQLServer_EntityFramework/assets/32194879/7840005e-6cbb-4cd1-9ffd-59e36be94c12)

