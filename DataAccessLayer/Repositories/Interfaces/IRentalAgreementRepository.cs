using Models.RentalAgreements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IRentalAgreementRepository
    {
        Task<List<GetRentalAgreementExpandedModel>> GetRentalAgreements();
        Task<List<GetRentalAgreementExpandedModel>> GetRentalAgreementsMine();
        Task<List<GetRentalAgreementExpandedModel>> GetRentalAgreementsMineAvailable();
        Task<List<GetRentalAgreementExpandedModel>> GetRentalAgreementsMineAvailable(Guid expositionId);
        Task<GetRentalAgreementModel> GetRentalAgreement(Guid id);
        Task<GetRentalAgreementModel> PostRentalAgreement(PostRentalAgreementModel postRentalAgreementModel);
        Task<GetRentalAgreementModel> PutRentalAgreement(Guid id, PutRentalAgreementModel putRentalAgreementModel);
    }
}
