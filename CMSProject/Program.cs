
using CMSProject.Application.Interfaces;
using CMSProject.Application.Services;
using CMSProject.Core.Domain.Interfaces;
using CMSProject.Infrastructure.Cache;
using CMSProject.Infrastructure.Persistence;
using CMSProject.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CMSProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddDbContext<AppDbContext>(options =>options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("CMSProject.Infrastructure")));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<IContentService, ContentService>();
            //repo
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IContentRepository, ContentRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            //service
            builder.Services.AddScoped<IContentService, ContentService>();
            builder.Services.AddScoped<IUserService, UserService>();

            //cache
            builder.Services.AddScoped<ICacheService, RedisCacheService>();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
                options.InstanceName = "CMSProject";
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
