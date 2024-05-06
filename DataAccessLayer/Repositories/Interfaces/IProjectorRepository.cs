using Models.Projectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IProjectorRepository
    {
        Task<List<GetProjectorModel>> GetProjectors();
        Task<GetProjectorModel> GetProjector(Guid id);
        Task<GetProjectorModel> PostProjector(PostProjectorModel postProjectorModel);
        Task<GetProjectorModel> PutProjector(Guid id, PutProjectorModel putProjectorModel);
        Task<GetProjectorModel> PatchProjector(Guid id, PatchProjectorModel patchProjectorModel);
    }
}
