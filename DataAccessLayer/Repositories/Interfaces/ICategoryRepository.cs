using Models.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<GetCategoryModel>> GetCategories();
        Task<GetCategoryModel> GetCategory(Guid id);
        Task<GetCategoryModel> PostCategory(PostCategoryModel postCategoryModel);
    }
}
