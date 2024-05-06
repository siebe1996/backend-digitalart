using Models.Categories;
using Models.Likes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface ILikeRepository
    {
        Task<List<GetLikeModel>> GetLikes();
        Task<List<GetLikeModel>> GetLikesMine();
        Task<GetLikeModel> GetLike(Guid artpieceId);
        Task<GetLikeModel> PostLike(PostLikeModel postLikeModel);
    }
}
