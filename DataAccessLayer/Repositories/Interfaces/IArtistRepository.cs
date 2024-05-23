using Models.Artists;
using Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IArtistRepository
    {
        Task<List<GetArtistModel>> GetArtists();
        Task<GetArtistModel> GetArtist(Guid id);
        Task<GetArtistModel> GetArtist();
        Task<GetArtistModel> PostArtist(PostArtistModel postArtistModel, string ipAddress);
    }
}
