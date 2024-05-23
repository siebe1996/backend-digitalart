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
        Task<List<GetArtpieceModel>> GetArtpiecesExposition(Guid expositionId);
        Task<List<GetArtpieceModel>> GetArtpiecesUnratedByCategoryIds(List<Guid> categoryIds);
        Task<List<GetArtpieceExpandedModel>> GetArtpiecesUnrated();
        Task<List<GetArtpieceExpandedModel>> GetArtpiecesRatedLikedByArtistNames(string artistNames);
        Task<List<GetArtpieceExpandedModel>> GetArtpiecesRatedLiked();
        Task<GetArtpieceModel> PostArtpiece(PostArtpieceModel postArtpieceModel);
    }
}
