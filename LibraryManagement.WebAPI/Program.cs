using LibraryManagement.Application.Features.Book.Commands;
using LibraryManagement.Application.Features.Book.Queries;
using LibraryManagement.Application.IRepositories;
using LibraryManagement.Infrastructure.Repository;
using LibraryManagement.Infrastructure;
using LibraryManagement.WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using LibraryManagement.Application.Responses;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryDatabase")));

builder.Services.AddTransient<IBookRepository, BookRepository>();
builder.Services.AddTransient<IAuthenticationService, MockAuthenticationService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(GetBookByISBNQuery).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(DeleteBookCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(UpdateBookCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(DeleteBookCommand).Assembly));

builder.Services.AddMemoryCache();

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey");
var issuer = jwtSettings.GetValue<string>("Issuer");
var audience = jwtSettings.GetValue<string>("Audience");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
          .AddJwtBearer(o =>
          {
              o.RequireHttpsMetadata = false;
              o.SaveToken = false;
              o.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuerSigningKey = true,
                  ValidateIssuer = false,
                  ValidateAudience = false,
                  ValidateLifetime = true,
                  ClockSkew = TimeSpan.Zero,
                  ValidIssuer = issuer,
                  ValidAudience = audience,
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
              };
              o.Events = new JwtBearerEvents()
              {
                  
                  OnAuthenticationFailed = context =>
                  {
                      var result = "";
                      if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                      {
                          context.Response.Headers.Add("Token-Expired", "true");
                          result = JsonConvert.SerializeObject(new Response<string>("Token Expired", false, "Token Expired"));
                      }
                      else
                      {
                          result = JsonConvert.SerializeObject(new Response<string>("Token Authentication Failed", false, context.Exception.ToString()));
                      }
                      context.NoResult();
                      context.Response.StatusCode = 900;
                      context.Response.ContentType = "application/json";
                      return context.Response.WriteAsync(result);
                  },
                  OnChallenge = context =>
                  {
                      context.HandleResponse();
                      context.Response.StatusCode = 401;
                      context.Response.ContentType = "application/json";
                      var result = JsonConvert.SerializeObject(new Response<string>("you are  not authorized", false, "Not Authorized"));
                      return context.Response.WriteAsync(result);
                  },
                  OnForbidden = context =>
                  {
                      context.Response.StatusCode = 403;
                      context.Response.ContentType = "application/json";
                      var result = JsonConvert.SerializeObject(new Response<string>("You are not authorized to access this resource", false, "Forbidden"));
                      return context.Response.WriteAsync(result);
                  },
              };
          });

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Library Managment", Version = "v1" });

    // Configure JWT authentication for Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

// Use authentication before authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
