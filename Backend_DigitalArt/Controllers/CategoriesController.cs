using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Categories;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <summary>
        /// Gets a list of categories.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 119ms
        /// </remarks>
        /// <returns>A list of categories.</returns>
        [HttpGet]
        public async Task<ActionResult<List<GetCategoryModel>>> GetCategories()
        {
            var models = await _categoryRepository.GetCategories();
            return models == null ? NotFound() : Ok(models);
        }
    }
}
