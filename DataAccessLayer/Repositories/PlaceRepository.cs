using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Models.Places;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class PlaceRepository : IPlaceRepository
    {
        private readonly Backend_DigitalArtContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _user;

        public PlaceRepository(Backend_DigitalArtContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<GetPlaceModel> GetPlace(Guid id)
        {
            var place = await _context.Places
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new GetPlaceModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Country = x.Country,
                    Province = x.Province,
                    City = x.City,
                    PostalCode = x.PostalCode,
                    Street = x.Street,
                    Address = x.Address,
                    Latitude = x.Coordinates.Y,
                    Longitude = x.Coordinates.X,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                })
                .FirstOrDefaultAsync();
            if (place == null)
            {
                throw new NotFoundException("Place Not Found");
            }
            return place;
        }

        public async Task<List<GetPlaceModel>> GetPlaces()
        {
            List<GetPlaceModel> places = await _context.Places.Select(x => new GetPlaceModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Country = x.Country,
                Province = x.Province,
                City = x.City,
                PostalCode = x.PostalCode,
                Street = x.Street,
                Address = x.Address,
                Latitude = x.Coordinates.Y,
                Longitude = x.Coordinates.X,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            }).AsNoTracking()
            .ToListAsync();

            return places;
        }

        public async Task<List<GetPlaceModel>> GetPlacesMine()
        {
            Guid userId = new Guid(_user.Identity.Name);

            List<GetPlaceModel> places = await _context.Places
                .Where(x => x.ExhibitorPlaces.Any(ep => ep.ExhibitorId == userId))
                .Select(x => new GetPlaceModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Country = x.Country,
                Province = x.Province,
                City = x.City,
                PostalCode = x.PostalCode,
                Street = x.Street,
                Address = x.Address,
                Latitude = x.Coordinates.Y,
                Longitude = x.Coordinates.X,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
            }).AsNoTracking()
            .ToListAsync();

            return places;
        }

        public async Task<GetPlaceModel> PostPlace(PostPlaceModel postPlaceModel)
        {
            var place = new Place
            {
                Name = postPlaceModel.Name,
                Description = postPlaceModel.Description,
                Country = postPlaceModel.Country,
                Province = postPlaceModel.Province,
                City = postPlaceModel.City,
                PostalCode = postPlaceModel.PostalCode,
                Street = postPlaceModel.Street,
                Address = postPlaceModel.Address,
                Coordinates = new Point(postPlaceModel.Longitude, postPlaceModel.Latitude),
            };

            _context.Places.Add(place);
            await _context.SaveChangesAsync();

            var getPlaceModel = new GetPlaceModel
            {
                Id = place.Id,
                Name = place.Name,
                Description = place.Description,
                Country = place.Country,
                Province = place.Province,
                City = place.City,
                PostalCode = place.PostalCode,
                Street = place.Street,
                Address = place.Address,
                Latitude = place.Coordinates.Y,
                Longitude = place.Coordinates.X,
                CreatedAt = place.CreatedAt,
                UpdatedAt = place.UpdatedAt,
            };
            return getPlaceModel;
        }
    }
}
