using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Likes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly Backend_DigitalArtContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public LikeRepository(Backend_DigitalArtContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<GetLikeModel> GetLike(Guid artpieceId)
        {
            Guid userId = new Guid(_user.Identity.Name);

            var like = await _context.Likes
                             .Where(l => l.UserId == userId && l.ArtpieceId == artpieceId)
                             .Select(l => new GetLikeModel
                             {
                                 UserId = l.UserId,
                                 ArtpieceId = l.ArtpieceId,
                                 Liked = l.Liked,
                                 CreatedAt = l.CreatedAt,
                                 UpdatedAt = l.UpdatedAt
                             })
                             .FirstOrDefaultAsync();

            if (like == null)
            {
                // Optionally throw an exception or handle the case where there is no like
                throw new Exception("Like not found");
            }

            return like;
        }

        public async Task<List<GetLikeModel>> GetLikes()
        {
            List<GetLikeModel> likes = await _context.Likes.Select(x => new GetLikeModel
            {
                UserId = x.UserId,
                ArtpieceId = x.ArtpieceId,
                Liked = x.Liked,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).AsNoTracking()
            .ToListAsync();

            return likes;
        }

        public async Task<List<GetLikeModel>> GetLikesMine()
        {
            Guid userId = new Guid(_user.Identity.Name);
            List<GetLikeModel> likes = await _context.Likes
                .Where(l => l.UserId == userId)
                .Select(x => new GetLikeModel
            {
                UserId = x.UserId,
                ArtpieceId = x.ArtpieceId,
                Liked = x.Liked,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).AsNoTracking()
            .ToListAsync();

            return likes;
        }

        public async Task<GetLikeModel> PostLike(PostLikeModel postLikeModel)
        {
            Guid userId = new Guid(_user.Identity.Name);
            var existingLike = await _context.Likes
                                     .FirstOrDefaultAsync(l => l.UserId == userId && l.ArtpieceId == postLikeModel.ArtpieceId);

            if (existingLike != null)
            {
                // If an entry exists, update it
                existingLike.Liked = postLikeModel.Liked;  // Assuming you have an UpdatedAt field that you wish to update
            }
            else
            {
                // Create a new like if it doesn't exist
                var newLike = new Like
                {
                    UserId = userId,
                    ArtpieceId = postLikeModel.ArtpieceId,
                    Liked = postLikeModel.Liked
                };
                _context.Likes.Add(newLike);
            }
            await _context.SaveChangesAsync();
            return new GetLikeModel
            {
                UserId = userId,
                ArtpieceId = postLikeModel.ArtpieceId,
                Liked = postLikeModel.Liked,
                CreatedAt = existingLike?.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
