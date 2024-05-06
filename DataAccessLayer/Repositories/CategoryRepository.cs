using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Artpieces;
using Models.Categories;
using Models.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly Backend_DigitalArtContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public CategoryRepository(Backend_DigitalArtContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<GetCategoryModel> GetCategory(Guid id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new GetCategoryModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
                .FirstOrDefaultAsync();
            if (category == null)
            {
                throw new NotFoundException("Category Not Found");
            }
            return category;
        }

        public async Task<List<GetCategoryModel>> GetCategories()
        {
            List<GetCategoryModel> categories = await _context.Categories.Select(x => new GetCategoryModel
            {
                Id = x.Id,
                Description = x.Description,
                Name = x.Name,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            }).AsNoTracking()
            .ToListAsync();

            return categories;
        }

        public async Task<GetCategoryModel> PostCategory(PostCategoryModel postCategoryModel)
        {
            var category = new Category
            {
                Name = postCategoryModel.Name,
                Description = postCategoryModel.Description,
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var getCategoryModel = new GetCategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
            };
            return getCategoryModel;
        }
    }
}
