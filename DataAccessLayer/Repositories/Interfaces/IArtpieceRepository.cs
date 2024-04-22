using Models.Artpieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IArtpieceRepository
    {
        Task<GetArtpieceModel> GetArtpiece(Guid id);
        Task<GetArtpieceModel> PostArtpiece(PostArtpieceModel postArtpieceModel);
    }
}
