using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Artpieces;
using Models.Categories;
using Models.Places;
using Models.Projectors;
using Models.RentalAgreements;
using Models.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProjectorRepository _projectorRepository;
        private readonly IRentalAgreementRepository _rentalAgreementRepository;

        public AdminController(IUserRepository userRepository, IPlaceRepository placeRepository, ICategoryRepository categoryRepository, IProjectorRepository projectorRepository, IRentalAgreementRepository rentalAgreementRepository)
        {
            _userRepository = userRepository;
            _placeRepository = placeRepository;
            _categoryRepository = categoryRepository;
            _projectorRepository = projectorRepository;
            _rentalAgreementRepository = rentalAgreementRepository;
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"];
            }
            else
            {
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
        }

        /// <summary>
        /// Gets a list of admins.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 145ms
        /// </remarks>
        /// <returns>A list of admins.</returns>
        [HttpGet("Admins")]
        public async Task<ActionResult<List<GetUserModel>>> GetAdmins()
        {
            var models = await _userRepository.GetAdmins();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets the authenticated admin.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 132ms
        /// </remarks>
        /// <returns>The authenticated admin.</returns>
        [HttpGet("Admins/Me")]
        public async Task<ActionResult<GetUserModel>> GetAdmin()
        {
            var model = await _userRepository.GetAdmin();
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Gets an admin by ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 158ms
        /// </remarks>
        /// <param name="id">The ID of the admin.</param>
        /// <returns>An admin object.</returns>
        [HttpGet("Admins/{id}")]
        public async Task<ActionResult<GetUserModel>> GetAdmin(Guid id)
        {
            var model = await _userRepository.GetAdmin(id);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Creates a new admin.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 198ms
        /// </remarks>
        /// <param name="postUserModel">The admin to create.</param>
        /// <returns>The created admin.</returns>
        [HttpPost("Admins")]
        public async Task<ActionResult<GetUserModel>> PostAdmin(PostUserModel postUserModel)
        {
            var model = await _userRepository.PostAdmin(postUserModel, IpAddress());
            return CreatedAtAction("GetAdmin", new { id = model.Id }, model);
        }

        /// <summary>
        /// Gets a category by ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 122ms
        /// </remarks>
        /// <param name="id">The ID of the category.</param>
        /// <returns>A category object.</returns>
        [HttpGet("Categories/{id}")]
        public async Task<ActionResult<GetCategoryModel>> GetCategory(Guid id)
        {
            var model = await _categoryRepository.GetCategory(id);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 174ms
        /// </remarks>
        /// <param name="postCategoryModel">The category to create.</param>
        /// <returns>The created category.</returns>
        [HttpPost("Categories")]
        public async Task<ActionResult<GetCategoryModel>> PostCategory(PostCategoryModel postCategoryModel)
        {
            var model = await _categoryRepository.PostCategory(postCategoryModel);
            return CreatedAtAction("GetCategory", new { id = model.Id }, model);
        }

        /// <summary>
        /// Updates a category.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 149ms
        /// </remarks>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="putModel">The updated category data.</param>
        /// <returns>The updated category.</returns>
        [HttpPut("Categories/{id}")]
        public async Task<ActionResult<GetCategoryModel>> PutCategory([FromRoute] Guid id, [FromBody] PutCategoryModel putModel)
        {
            var model = await _categoryRepository.PutCategory(id, putModel);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Gets a list of projectors.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 134ms
        /// </remarks>
        /// <returns>A list of projectors.</returns>
        [HttpGet("Projectors")]
        public async Task<ActionResult<List<GetProjectorModel>>> GetProjectors()
        {
            var models = await _projectorRepository.GetProjectors();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a list of reserved projectors.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 116ms
        /// </remarks>
        /// <returns>A list of reserved projectors.</returns>
        [HttpGet("Projectors/Reserved")]
        public async Task<ActionResult<List<Guid>>> GetReservedProjectors()
        {
            var models = await _projectorRepository.GetReservedProjectors();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a projector by ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 123ms
        /// </remarks>
        /// <param name="id">The ID of the projector.</param>
        /// <returns>A projector object.</returns>
        [HttpGet("Projectors/{id}")]
        public async Task<ActionResult<GetProjectorModel>> GetProjector(Guid id)
        {
            var model = await _projectorRepository.GetProjector(id);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Creates a new projector.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 187ms
        /// </remarks>
        /// <param name="postProjectorModel">The projector to create.</param>
        /// <returns>The created projector.</returns>
        [HttpPost("Projectors")]
        public async Task<ActionResult<GetProjectorModel>> PostProjector(PostProjectorModel postProjectorModel)
        {
            var model = await _projectorRepository.PostProjector(postProjectorModel);
            return CreatedAtAction("GetProjector", new { id = model.Id }, model);
        }

        /// <summary>
        /// Updates a projector.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 154ms
        /// </remarks>
        /// <param name="id">The ID of the projector to update.</param>
        /// <param name="putProjectorModel">The updated projector data.</param>
        /// <returns>The updated projector.</returns>
        [HttpPut("Projectors/{id}")]
        public async Task<ActionResult<GetProjectorModel>> PutProjector([FromRoute] Guid id, [FromBody] PutProjectorModel putProjectorModel)
        {
            var model = await _projectorRepository.PutProjector(id, putProjectorModel);
            return model != null ? Ok(model) : NotFound();
        }

        /// <summary>
        /// Partially updates a projector.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 142ms
        /// </remarks>
        /// <param name="id">The ID of the projector to update.</param>
        /// <param name="patchProjectorModel">The updated projector data.</param>
        /// <returns>The updated projector.</returns>
        [HttpPatch("Projectors/{id}")]
        public async Task<ActionResult<GetProjectorModel>> PatchProjector([FromRoute] Guid id, [FromBody] PatchProjectorModel patchProjectorModel)
        {
            var model = await _projectorRepository.PatchProjector(id, patchProjectorModel);
            return model != null ? Ok(model) : NotFound();
        }

        /// <summary>
        /// Gets a list of rental agreements.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 155ms
        /// </remarks>
        /// <returns>A list of rental agreements.</returns>
        [HttpGet("Rentalagreements")]
        public async Task<ActionResult<List<GetRentalAgreementModel>>> GetRentalAgreements()
        {
            var models = await _rentalAgreementRepository.GetRentalAgreements();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a rental agreement by ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 167ms
        /// </remarks>
        /// <param name="id">The ID of the rental agreement.</param>
        /// <returns>A rental agreement object.</returns>
        [HttpGet("Rentalagreements/{id}")]
        public async Task<ActionResult<GetRentalAgreementModel>> GetRentalAgreement(Guid id)
        {
            var model = await _rentalAgreementRepository.GetRentalAgreement(id);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Updates a rental agreement.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 163ms
        /// </remarks>
        /// <param name="id">The ID of the rental agreement to update.</param>
        /// <param name="putRentalAgreementModel">The updated rental agreement data.</param>
        /// <returns>The updated rental agreement.</returns>
        [HttpPut("Rentalagreements/{id}")]
        public async Task<ActionResult<GetRentalAgreementModel>> PutRentalAgreement([FromRoute] Guid id, [FromBody] PutRentalAgreementModel putRentalAgreementModel)
        {
            var model = await _rentalAgreementRepository.PutRentalAgreement(id, putRentalAgreementModel);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Gets a list of places for an exhibitor.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 129ms
        /// </remarks>
        /// <param name="rentalagreementId">The ID of the rental agreement.</param>
        /// <returns>A list of places.</returns>
        [HttpGet("Rentalagreements/{rentalagreementId}/Exhibitors/Places")]
        public async Task<ActionResult<List<GetPlaceModel>>> GetPlacesExhibitor(Guid rentalagreementId)
        {
            var models = await _placeRepository.GetPlacesExhibitor(rentalagreementId);
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets available projectors by date range including current.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 141ms
        /// </remarks>
        /// <param name="rentalagreementId">The ID of the rental agreement.</param>
        /// <param name="startDate">The start date for availability.</param>
        /// <param name="endDate">The end date for availability.</param>
        /// <returns>A list of available projectors.</returns>
        [HttpGet("RentalAgreements/{rentalagreementId}/Projectors/Available/Search")]
        public async Task<ActionResult<List<GetArtpieceModel>>> GetAvialableProjectorsByDatesWithCurrent([FromRoute] Guid rentalagreementId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("startDate cannot be later than endDate.");
            }
            var models = await _projectorRepository.GetAvailableProjectorsByDatesWithCurrent(rentalagreementId, startDate, endDate);
            return models == null ? NotFound() : Ok(models);
        }
    }
}
