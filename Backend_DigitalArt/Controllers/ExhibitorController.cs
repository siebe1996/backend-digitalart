using Backend_DigitalAr.Services.Implementations;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Artpieces;
using Models.Expositions;
using Models.Places;
using Models.RentalAgreements;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Authorize(Roles = "Exhibitor")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ExhibitorController : ControllerBase
    {
        private readonly IPlaceRepository _placeRepository;
        private readonly IRentalAgreementRepository _rentalAgreementRepository;
        private readonly IProjectorRepository _projectorRepository;
        private readonly IExpositionRepository _expositionRepository;
        private readonly IArtpieceRepository _artpieceRepository;
        private readonly ExpositionService _expositionService;

        public ExhibitorController(IPlaceRepository placeRepository, IRentalAgreementRepository rentalAgreementRepository, IProjectorRepository projectorRepository, IExpositionRepository expositionRepository, IArtpieceRepository artpieceRepository, ExpositionService expositionService)
        {
            _placeRepository = placeRepository;
            _rentalAgreementRepository = rentalAgreementRepository;
            _projectorRepository = projectorRepository;
            _expositionRepository = expositionRepository;
            _expositionService = expositionService;
            _artpieceRepository = artpieceRepository;
        }

        /// <summary>
        /// Gets a list of the authenticated exhibitor's places.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 133ms
        /// </remarks>
        /// <returns>A list of places.</returns>
        [HttpGet("Places/Mine")]
        public async Task<ActionResult<List<GetPlaceModel>>> GetPlacesMine()
        {
            var models = await _placeRepository.GetPlacesMine();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a place by ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 121ms
        /// </remarks>
        /// <param name="id">The ID of the place.</param>
        /// <returns>A place object.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GetPlaceModel>> GetPlace(Guid id)
        {
            var model = await _placeRepository.GetPlace(id);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Creates a new place.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 164ms
        /// </remarks>
        /// <param name="postPlaceModel">The place to create.</param>
        /// <returns>The created place.</returns>
        [HttpPost("Places")]
        public async Task<ActionResult<GetPlaceModel>> PostPlace(PostPlaceModel postPlaceModel)
        {
            var model = await _placeRepository.PostPlace(postPlaceModel);
            return CreatedAtAction("GetPlace", new { id = model.Id }, model);
        }

        /// <summary>
        /// Updates a place.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 153ms
        /// </remarks>
        /// <param name="id">The ID of the place to update.</param>
        /// <param name="putPlaceModel">The updated place data.</param>
        /// <returns>The updated place.</returns>
        [HttpPut("Places/{id}")]
        public async Task<ActionResult<GetPlaceModel>> PutPlace([FromRoute] Guid id, [FromBody] PutPlaceModel putPlaceModel)
        {
            var model = await _placeRepository.PutPlace(id, putPlaceModel);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Gets a list of the authenticated exhibitor's rental agreements.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 139ms
        /// </remarks>
        /// <returns>A list of rental agreements.</returns>
        [HttpGet("Rentalagreements/Mine")]
        public async Task<ActionResult<List<GetRentalAgreementModel>>> GetRentalAgreementsMine()
        {
            var models = await _rentalAgreementRepository.GetRentalAgreementsMine();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a list of available rental agreements for the authenticated exhibitor.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 145ms
        /// </remarks>
        /// <returns>A list of available rental agreements.</returns>
        [HttpGet("Rentalagreements/Mine/Available")]
        public async Task<ActionResult<List<GetRentalAgreementModel>>> GetRentalAgreementsMineAvailable()
        {
            var models = await _rentalAgreementRepository.GetRentalAgreementsMineAvailable();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a list of available rental agreements for the authenticated exhibitor by exposition ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 144ms
        /// </remarks>
        /// <param name="expositionId">The ID of the exposition.</param>
        /// <returns>A list of available rental agreements.</returns>
        [HttpGet("Rentalagreements/Mine/Available/{expositionId}")]
        public async Task<ActionResult<List<GetRentalAgreementModel>>> GetRentalAgreementsMineAvailable(Guid expositionId)
        {
            var models = await _rentalAgreementRepository.GetRentalAgreementsMineAvailable(expositionId);
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a rental agreement by ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 128ms
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
        /// Creates a new rental agreement.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 171ms
        /// </remarks>
        /// <param name="postRentalAgreementModel">The rental agreement to create.</param>
        /// <returns>The created rental agreement.</returns>
        [HttpPost("Rentalagreements")]
        public async Task<ActionResult<GetRentalAgreementModel>> PostRentalAgreement(PostRentalAgreementModel postRentalAgreementModel)
        {
            var model = await _rentalAgreementRepository.PostRentalAgreement(postRentalAgreementModel);
            return CreatedAtAction("GetRentalAgreement", new { id = model.Id }, model);
        }

        /// <summary>
        /// Gets available projectors by date range.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 136ms
        /// </remarks>
        /// <param name="startDate">The start date for availability.</param>
        /// <param name="endDate">The end date for availability.</param>
        /// <returns>A list of available projectors.</returns>
        [HttpGet("Projectors/Available/Search")]
        public async Task<ActionResult<List<GetArtpieceModel>>> GetAvialableProjectorsByDates([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("startDate cannot be later than endDate.");
            }
            var models = await _projectorRepository.GetAvailableProjectorsByDates(startDate, endDate);
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a list of the authenticated exhibitor's expositions.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 132ms
        /// </remarks>
        /// <returns>A list of expositions.</returns>
        [HttpGet("Expositions/Mine")]
        public async Task<ActionResult<List<GetExpositionExpandedModel>>> GetExpositionsMine()
        {
            var models = await _expositionRepository.GetExpositionsMine();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets a list of active exposition IDs for the authenticated exhibitor.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 126ms
        /// </remarks>
        /// <returns>A list of active exposition IDs.</returns>
        [HttpGet("Expositions/Mine/Active/Ids")]
        public async Task<ActionResult<List<Guid>>> GetExpositionsIdsMineActive()
        {
            var models = await _expositionRepository.GetExpositionsIdsMineActive();
            return models == null ? NotFound() : Ok(models);
        }

        /// <summary>
        /// Gets an exposition by ID.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 125ms
        /// </remarks>
        /// <param name="id">The ID of the exposition.</param>
        /// <returns>An exposition object.</returns>
        [HttpGet("Expositions/{id}")]
        public async Task<ActionResult<GetExpositionModel>> GetExposition(Guid id)
        {
            var model = await _expositionRepository.GetExposition(id);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Gets a list of artpieces for a specific exposition.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 129ms
        /// </remarks>
        /// <param name="id">The ID of the exposition.</param>
        /// <returns>A list of artpieces.</returns>
        [HttpGet("Expositions/{id}/Artpieces")]
        public async Task<ActionResult<List<GetArtpieceModel>>> GetArtpiecesExpositions(Guid id)
        {
            var model = await _artpieceRepository.GetArtpiecesExposition(id);
            return model == null ? NotFound() : Ok(model);
        }

        /// <summary>
        /// Creates a new exposition.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 175ms
        /// </remarks>
        /// <param name="postExpositionModel">The exposition to create.</param>
        /// <returns>The created exposition.</returns>
        [HttpPost("Expositions")]
        public async Task<ActionResult<GetExpositionModel>> PostExpostition(PostExpositionModel postExpositionModel)
        {
            var model = await _expositionRepository.PostExposition(postExpositionModel);
            return CreatedAtAction("GetExposition", new { id = model.Id }, model);
        }

        /// <summary>
        /// Updates an exposition.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 149ms
        /// </remarks>
        /// <param name="id">The ID of the exposition to update.</param>
        /// <param name="putExpositionModel">The updated exposition data.</param>
        /// <returns>The updated exposition.</returns>
        [HttpPut("Expositions/{id}")]
        public async Task<ActionResult<GetExpositionModel>> PutExpostition([FromRoute] Guid id, [FromBody] PutExpositionModel putExpositionModel)
        {
            var model = await _expositionRepository.PutExposition(id, putExpositionModel);
            return CreatedAtAction("GetExposition", new { id = model.Id }, model);
        }

        /// <summary>
        /// Partially updates an exposition.
        /// </summary>
        /// <remarks>
        /// Average Response Time: 153ms
        /// </remarks>
        /// <param name="id">The ID of the exposition to update.</param>
        /// <param name="patchExpositionModel">The updated exposition data.</param>
        /// <returns>The updated exposition.</returns>
        [HttpPatch("Expositions/{id}")]
        public async Task<ActionResult<GetExpositionModel>> PatchExposition([FromRoute] Guid id, [FromBody] PatchExpositionModel patchExpositionModel)
        {
            var model = await _expositionRepository.PatchExposition(id, patchExpositionModel);
            await _expositionService.UpdateExpositionStatus(id, patchExpositionModel);
            return model == null ? NotFound() : Ok(model);
        }
    }
}
