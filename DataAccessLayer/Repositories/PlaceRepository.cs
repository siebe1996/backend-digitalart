using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Places;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        //toDo fix forbiddenexceptions

        public async Task<GetPlaceModel> GetPlace(Guid id)
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }
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

        public async Task<List<GetPlaceModel>> GetPlacesWithActiveExpositions()
        {
            List<GetPlaceModel> places = await _context.Places
                .Where(p => p.RentalAgreements
                    .Any(ra => ra.Projector.Exposition != null && ra.Projector.Exposition.Active))
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
                .Distinct()
                .AsNoTracking()
                .ToListAsync();

            return places;
        }



        public async Task<List<GetPlaceModel>> GetPlacesMine()
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }
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

        public async Task<List<GetPlaceModel>> GetPlacesExhibitor(Guid rentalagreementId)
        {
            bool hasAccess = _user.IsInRole("Admin");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }

            var exhibitorId = await _context.RentalAgreements
            .Where(ra => ra.Id == rentalagreementId)
            .Select(
                ra => ra.Place.ExhibitorPlaces.Select(ep => ep.ExhibitorId).FirstOrDefault()
            ).SingleOrDefaultAsync();

            if (exhibitorId == default)
            {
                throw new NotFoundException("Exhibitor not found for the given Rental Agreement.");
            }

            List<GetPlaceModel> places = await _context.Places
                .Where(x => x.ExhibitorPlaces.Any(ep => ep.ExhibitorId == exhibitorId))
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
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }
            Guid userId = new Guid(_user.Identity.Name);
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

            var exhibitorPlace = new ExhibitorPlace
            {
                ExhibitorId = userId,
                PlaceId = place.Id
            };

            _context.ExhibitorPlaces.Add(exhibitorPlace);
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

        public async Task<GetPlaceModel> PutPlace(Guid id, PutPlaceModel putPlaceModel)
        {
            bool hasAccess = _user.IsInRole("Exhibitor");
            if (!hasAccess)
            {
                throw new ForbiddenException("Not Allowed");
            }

            var place = await _context.Places.FindAsync(id);
            if (place == null)
            {
                throw new NotFoundException("Place Not Found");
            }

            place.Name = putPlaceModel.Name;
            place.Description = putPlaceModel.Description;
            place.Country = putPlaceModel.Country;
            place.Province = putPlaceModel.Province;
            place.City = putPlaceModel.City;
            place.PostalCode = putPlaceModel.PostalCode;
            place.Street = putPlaceModel.Street;
            place.Address = putPlaceModel.Address ;
            place.Coordinates = new Point(putPlaceModel.Longitude, putPlaceModel.Latitude);

            _context.Places.Update(place);
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

        public async Task<GetPlaceModel> PatchPlace(Guid id, PatchPlaceModel patchPlaceModel)
        {
            var place = await _context.Places.FindAsync(id);
            if (place == null)
            {
                throw new NotFoundException("Place Not Found");
            }

            place.Name = patchPlaceModel.Name ?? place.Name;
            place.Description = patchPlaceModel.Description ?? place.Description;
            place.Country = patchPlaceModel.Country ?? place.Country;
            place.Province = patchPlaceModel.Province ?? place.Province;
            place.City = patchPlaceModel.City ?? place.City;
            place.PostalCode = patchPlaceModel.PostalCode ?? place.PostalCode;
            place.Street = patchPlaceModel.Street ?? place.Street;
            place.Address = patchPlaceModel.Address ?? place.Address;
            if (patchPlaceModel.Latitude.HasValue && patchPlaceModel.Longitude.HasValue)
            {
                place.Coordinates = new Point(patchPlaceModel.Longitude.Value, patchPlaceModel.Latitude.Value);
            }

            _context.Places.Update(place);
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
