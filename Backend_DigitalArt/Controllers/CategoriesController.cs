using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Categories;
using Models.Roles;

namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<ActionResult<GetCategoryModel>> GetCategories()
        {
            var models = await _categoryRepository.GetCategories();
            return models == null ? NotFound() : Ok(models);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetCategoryModel>> GetCategory(Guid Id)
        {
            var model = await _categoryRepository.GetCategory(Id);
            return model == null ? NotFound() : Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult<GetCategoryModel>> PostCategory(PostCategoryModel postCategoryModel)
        {
            var model = await _categoryRepository.PostCategory(postCategoryModel);
            return CreatedAtAction("GetCategory", new { id = model.Id }, model);
        }
    }
}
