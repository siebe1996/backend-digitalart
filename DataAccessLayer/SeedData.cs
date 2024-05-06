using Globals.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new Backend_DigitalArtContext(serviceProvider.GetRequiredService<DbContextOptions<Backend_DigitalArtContext>>());
            using UserManager<User> _userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            using RoleManager<Role> _roleManager = serviceProvider.GetService<RoleManager<Role>>();
            SeedRoles(_roleManager);
            SeedCategories(context);
            //SeedUsers(_userManager);
            /*SeedPlaces(context, _userManager);
            SeedReviews(context, _userManager);
            SeedAssessments(context, _userManager);*/
        }

        private static void SeedRoles(RoleManager<Role> _roleManager)
        {
            if (!_roleManager.RoleExistsAsync("User").Result)
            {
                _ = _roleManager.CreateAsync(new Role { Name = "User", Description = "Regular user role" }).Result;
            }

            if (!_roleManager.RoleExistsAsync("Artist").Result)
            {
                _ = _roleManager.CreateAsync(new Role { Name = "Artist", Description = "Artist role" }).Result;
            }

            if (!_roleManager.RoleExistsAsync("Exhibitor").Result)
            {
                _ = _roleManager.CreateAsync(new Role { Name = "Exhibitor", Description = "Exhibitor role" }).Result;
            }

            if (!_roleManager.RoleExistsAsync("Admin").Result)
            {
                _ = _roleManager.CreateAsync(new Role { Name = "Admin", Description = "Administrator role" }).Result;
            }
        }

        private static void SeedCategories(Backend_DigitalArtContext context)
        {
            var categories = new List<Category>
            {
                new Category { Name = "Painting", Description = "All forms of painting, including oil, acrylic, and watercolor.", CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow},
                new Category { Name = "Sculpture", Description = "Three-dimensional art created by shaping or combining materials.", CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow},
                new Category { Name = "Photography", Description = "Art of capturing light with a camera, typically via a digital sensor or film.", CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow},
                new Category { Name = "Digital Art", Description = "Artistic work or practice that uses digital technology as part of the creative or presentation process.", CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow},
                new Category { Name = "Mixed Media", Description = "Artworks composed from a combination of different media or materials.", CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow},
                new Category { Name = "Category 1", Description = "Description for Category 1", CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow},
                new Category { Name = "Category 2", Description = "Description for Category 2", CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow},
                new Category { Name = "Category 3", Description = "Description for Category 3", CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow}
            };

            foreach (var category in categories)
            {
                 // Check if the category already exists
                 if (!context.Set<Category>().Any(c => c.Name == category.Name))
                 {
                     context.Set<Category>().Add(category);
                 }
            }

            // Save all changes to the database
            context.SaveChanges();
        }

        private static void SeedUsers(UserManager<User> _userManager)
        {
            if (_userManager.FindByEmailAsync("admin@example.com").Result == null)
            {
                User adminUser = new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    DateOfBirth = DateTime.Parse("1990-01-01"),
                    Email = "admin@example.com",
                };
                _ = _userManager.CreateAsync(adminUser, "Azerty123!").Result;
                _ = _userManager.AddToRoleAsync(adminUser, "User").Result;
                _ = _userManager.AddToRoleAsync(adminUser, "Artist").Result;
                _ = _userManager.AddToRoleAsync(adminUser, "Exhibitor").Result;
                _ = _userManager.AddToRoleAsync(adminUser, "Admin").Result;
            }

            if (_userManager.FindByEmailAsync("user@example.com").Result == null)
            {
                User regularUser = new User
                {
                    FirstName = "Regular",
                    LastName = "User",
                    DateOfBirth = DateTime.Parse("1995-05-05"),
                    Email = "user@example.com",
                };
                _ = _userManager.CreateAsync(regularUser, "Azerty123!").Result;
                _ = _userManager.AddToRoleAsync(regularUser, "User").Result;
            }

            if (_userManager.FindByEmailAsync("artist@example.com").Result == null)
            {
                User user = new User
                {
                    FirstName = "Artist",
                    LastName = "User",
                    DateOfBirth = DateTime.Parse("1995-05-05"),
                    Email = "artist@example.com",
                };
                _ = _userManager.CreateAsync(user, "Azerty123!").Result;
                _ = _userManager.AddToRoleAsync(user, "User").Result;
                _ = _userManager.AddToRoleAsync(user, "Artist").Result;
            }

            if (_userManager.FindByEmailAsync("exhibitor@example.com").Result == null)
            {
                User user = new User
                {
                    FirstName = "Exhibitor",
                    LastName = "User",
                    DateOfBirth = DateTime.Parse("1995-05-05"),
                    Email = "exhibitor@example.com",
                };
                _ = _userManager.CreateAsync(user, "Azerty123!").Result;
                _ = _userManager.AddToRoleAsync(user, "User").Result;
                _ = _userManager.AddToRoleAsync(user, "Exhibitor").Result;
            }
        }
    }
}
