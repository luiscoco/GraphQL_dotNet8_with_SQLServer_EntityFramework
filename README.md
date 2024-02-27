# How to create .NET 8 GraphQL API with SQLServer and EntityFramework

## 1. Pull and run the SQLServer Docker image with Docker Desktop



## 2. Create a new .NET 8 WebAPI with Visual Studio 2022



## 3. Create the project structure



## 4. Add project dependencies 



## 5. Add the models



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




