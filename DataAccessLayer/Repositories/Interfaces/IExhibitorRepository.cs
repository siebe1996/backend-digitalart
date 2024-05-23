using Models.Artists;
using Models.Exhibitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IExhibitorRepository
    {
        Task<List<GetExhibitorModel>> GetExhibitors();
        Task<GetExhibitorModel> GetExhibitor(Guid id);
        Task<GetExhibitorModel> GetExhibitor();
        Task<GetExhibitorModel> PostExhibitor(PostExhibitorModel postModel, string ipAddress);
    }
}
