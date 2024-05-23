using DataAccessLayer.Repositories;
using DataAccessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Artpieces;
using Models.RentalAgreements;

namespace Backend_DigitalArt.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class RentalAgreementsController : ControllerBase
    {
        private readonly IRentalAgreementRepository _rentalAgreementRepository;
        public RentalAgreementsController(IRentalAgreementRepository rentalAgreementRepository)
        {
            _rentalAgreementRepository = rentalAgreementRepository;
        }
    }
}
