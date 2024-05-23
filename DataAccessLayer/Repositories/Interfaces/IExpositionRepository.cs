using Models.Expositions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IExpositionRepository
    {
        Task<List<GetExpositionModel>> GetExpositions();
        Task<List<GetExpositionExpandedModel>> GetExpositionsMine();
        Task<List<Guid>> GetExpositionsIdsMineActive();
        Task<GetExpositionModel> GetExposition(Guid id);
        Task<GetExpositionModel> PostExposition(PostExpositionModel postExpositionModel);
        Task<GetExpositionModel> PutExposition(Guid id, PutExpositionModel putExpositionModel);
        Task<GetExpositionModel> PatchExposition(Guid id, PatchExpositionModel patchExpositionModel);
    }
}
