using Models.Places;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IPlaceRepository
    {
        Task<List<GetPlaceModel>> GetPlaces();
        Task<List<GetPlaceModel>> GetPlacesMine();
        Task<GetPlaceModel> GetPlace(Guid id);
        Task<GetPlaceModel> PostPlace(PostPlaceModel postPlaceModel);
        Task<GetPlaceModel> PutPlace(Guid id, PutPlaceModel putPlaceModel);
        Task<GetPlaceModel> PatchPlace(Guid id, PatchPlaceModel patchPlaceModel);
    }
}
