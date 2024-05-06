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
        Task<List<GetArtpieceModel>> GetArtpieces();
        Task<List<GetArtpieceModel>> GetArtpiecesUnratedByCategoryIds(List<Guid> categoryIds);
        Task<List<GetArtpieceModel>> GetArtpiecesUnrated();
        Task<List<GetArtpieceModel>> GetArtpiecesRatedLikedByArtistNames(string artistNames);
        Task<List<GetArtpieceModel>> GetArtpiecesRatedLiked();
        Task<GetArtpieceModel> PostArtpiece(PostArtpieceModel postArtpieceModel);
    }
}
