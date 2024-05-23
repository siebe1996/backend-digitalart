using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Repositories.Interfaces;
using Globals.Entities;
using Globals.Enums;
using Globals.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.Artists;
using Models.Users;
using Serilog;

namespace DataAccessLayer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly AppSettings _appSettings;
        private readonly RoleManager<Role> _roleManager;
        private readonly Backend_DigitalArtContext _context;
        private readonly ClaimsPrincipal _user;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserRepository(Backend_DigitalArtContext backend_DigitalArtContext, SignInManager<User> signInManager, UserManager<User> userManager, IOptions<AppSettings> appSettings, RoleManager<Role> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _context = backend_DigitalArtContext;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _user = _httpContextAccessor.HttpContext.User;
        }

        public async Task<string> GeneratePasswordResetToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }
            try
            {
                string test = await _userManager.GeneratePasswordResetTokenAsync(user);
                return test;
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception("Error generating password reset token.", ex);
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            User user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            //IdentityResult result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.NewPassword);
            //this is because i dont use it from email otherwise it needs to be in model
            string resetToken = await GeneratePasswordResetToken(user);
            IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordModel.NewPassword);
            return result.Succeeded;
        }

        /*public async Task<bool> ForgotPassword(ForgotPasswordModel forgotPasswordModel)
        {
            User user = await _userManager.FindByEmailAsync(forgotPasswordModel.Email);
            if (user == null)
            {
                // User not found, handle accordingly (e.g., do not reveal this information to the user)
                return false;
            }

            string resetToken = await GeneratePasswordResetToken(user);

            // TODO: Send the reset token to the user (e.g., via email)

            return true;
        }*/

        public async Task<string> GenerateJwtToken(User user)
        {
            var roleNames = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim("Email", user.Email)/*,
                new Claim("UserName", user.UserName)*/
            };

            foreach (string roleName in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
            }

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Issuer = "Backend_DigitalArt Web API",
                Subject = new ClaimsIdentity(claims.ToArray()),
                Expires = DateTime.UtcNow.AddSeconds(5260800), //TOKEN JWT change for experation time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(64);

            //The refresh token expires time must be the same as the refresh token cookie expires time set in the SetTokenCookie method in UserController
            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomBytes),
                Expires = DateTime.UtcNow.AddMinutes(131490), //TOKEN REFRESH
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
            };
        }

        public async Task<PostAuthenticateResponseModel> Authenticate(PostAuthenticateRequestModel postAuthenticateRequestModel, string ipAddress)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == postAuthenticateRequestModel.Email);

            if (user == null)
            {
                throw new Exception("Invalid email.");
            }

            SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, postAuthenticateRequestModel.Password, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                throw new Exception("Invalid password.");
            }

            string jwtToken = await GenerateJwtToken(user);
            RefreshToken refreshToken = GenerateRefreshToken(ipAddress);

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var artistDescription = await _context.Artists
                .Where(a => a.Id == user.Id)
                .Select(a => a.Description)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var roles = await _userManager.GetRolesAsync(user);

            var response = new PostAuthenticateResponseModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                MimeTypeImageData = user.MimeTypeImageData,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Street = user.Street,
                Address = user.Address,
                Description = artistDescription,
                JwtToken = jwtToken,
                RefreshToken = refreshToken.Token,
                Roles = roles,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };

            return response;
        }


        public async Task<PostAuthenticateResponseModel> RenewToken(string token, string ipAddress)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(x => x.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                throw new Exception("no user was found with this token");
            }

            RefreshToken refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            //Refresh token is no longer active
            if (!refreshToken.IsActive)
            {
                throw new Exception("refresh token is expired");
            }

            //Replace old refresh token with a new one
            RefreshToken newRefreshToken = GenerateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            //Generate new jwt
            string jwtToken = await GenerateJwtToken(user);

            user.RefreshTokens.Add(refreshToken);

            await _userManager.UpdateAsync(user);

            return new PostAuthenticateResponseModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                MimeTypeImageData = user.MimeTypeImageData,
                //UserName = user.UserName,
                //Score = user.Score,
                //Gender = user.Gender,
                //PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Street = user.Street,
                Address = user.Address,
                JwtToken = jwtToken,
                RefreshToken = newRefreshToken.Token,
                Roles = await _userManager.GetRolesAsync(user),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task DeactivateToken(string token, string ipAddress)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(x => x.RefreshTokens.Any(x => x.Token == token));

            if (user == null)
            {
                throw new Exception("no user was found with this token");
            }

            RefreshToken refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            //Refresh token is no longer active
            if (!refreshToken.IsActive)
            {
                throw new Exception("refresh token is expired");
            }

            //Revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            await _userManager.UpdateAsync(user);
        }


        //toDo check if work with roles
        public async Task<List<GetUserModel>> GetUsers()
        {
            List<GetUserModel> users = await _context.Users.Select(x => new GetUserModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                DateOfBirth = x.DateOfBirth,
                Email = x.Email,
                ImageData = x.ImageData,
                MimeTypeImageData = x.MimeTypeImageData,
                Country = x.Country,
                Province = x.Province,
                City = x.City,
                PostalCode = x.PostalCode,
                Street = x.Street,
                Address = x.Address,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            }).AsNoTracking()
            .ToListAsync();

            return users;
        }

        public async Task<GetUserModel> GetUser(Guid id)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new NotFoundException("Not Found");
            }
            var roleNames = await _userManager.GetRolesAsync(user);

            return new GetUserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                MimeTypeImageData = user.MimeTypeImageData,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Street = user.Street,
                Address = user.Address,
                Roles = roleNames,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }


        public async Task<GetUserModel> GetUser()
        {
            //need to get id
            bool hasAccess = _user.IsInRole("User");
            if (hasAccess)
            {
                Guid userId = new Guid(_user.Identity.Name);
                var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
                if (user == null)
                {
                    throw new NotFoundException("Not Found");
                }
                var roleNames = await _userManager.GetRolesAsync(user);

                GetUserModel userModel = new GetUserModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    Email = user.Email,
                    ImageData = user.ImageData,
                    Country = user.Country,
                    Province = user.Province,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    Street = user.Street,
                    Address = user.Address,
                    Roles = roleNames,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                };

                return userModel;
            }
            else{
                throw new ForbiddenException("Not allowed");
            }
        }


        //toDo change this so it doesnt need user role
        public async Task<GetUserModel> PostUser(PostUserModel postUserModel, string ipAddress)
        {

            User user = new User();

            user.FirstName = postUserModel.FirstName;
            user.LastName = postUserModel.LastName;
            user.DateOfBirth = postUserModel.DateOfBirth;
            user.Email = postUserModel.Email;
            user.ImageData = postUserModel.ImageData;
            user.MimeTypeImageData = postUserModel.MimeTypeImageData;
            user.UserName = postUserModel.Email;
            user.Country = postUserModel.Country;
            user.Province = postUserModel.Province;
            user.City = postUserModel.City;
            user.PostalCode = postUserModel.PostalCode;
            user.Street = postUserModel.Street;
            user.Address = postUserModel.Address;

            var createResult = await _userManager.CreateAsync(user, postUserModel.Password);
            if (!createResult.Succeeded)
            {
                throw new Exception($"User creation failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!roleResult.Succeeded)
            {
                // Handle failure: possibly throw an exception or return an error response
                throw new Exception($"Failed to add user to role User: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }

            
            await _context.SaveChangesAsync();
            var roleNames = await _userManager.GetRolesAsync(user);
            return new GetUserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                MimeTypeImageData = user.MimeTypeImageData,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Street = user.Street,
                Address = user.Address,
                Roles = roleNames,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };
        }

        public async Task<GetUserModel> PutUser(Guid id, PutUserModel putUserModel)
        {
            User user = await _context.Users.FirstOrDefaultAsync(p => p.Id == id);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            /*if (putUserModel.UserName != null)
            {
                user.UserName = putUserModel.UserName;
            }*/

            if (putUserModel.DateOfBirth != null)
            {
                user.DateOfBirth = (DateTime)putUserModel.DateOfBirth;
            }

            if (putUserModel.ImageData != null)
            {
                user.ImageData = putUserModel.ImageData;
                user.MimeTypeImageData = putUserModel.MimeTypeImageData;
            }

            await _context.SaveChangesAsync();

            GetUserModel usermodel = new GetUserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                MimeTypeImageData = user.MimeTypeImageData,
                //UserName = user.UserName,
                //Score = user.Score,
                //Gender = user.Gender,
                //PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Street = user.Street,
                Address = user.Address,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            
            return usermodel;
        }

        public async Task<List<GetUserModel>> GetAdmins()
        {
            bool hasAccessAdmin = _user.IsInRole("Admin");
            if (!hasAccessAdmin)
            {
                throw new ForbiddenException("Not Allowed");
            }


            var adminRoleId = await _roleManager.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefaultAsync();

            var adminUsers = await _context.Users
            .Where(u => _context.UserRoles
            .Any(ur => ur.UserId == u.Id && ur.RoleId == adminRoleId))
            .Select(u => new GetUserModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                DateOfBirth = u.DateOfBirth,
                Email = u.Email,
                ImageData = u.ImageData,
                MimeTypeImageData = u.MimeTypeImageData,
                Country = u.Country,
                Province = u.Province,
                City = u.City,
                PostalCode = u.PostalCode,
                Street = u.Street,
                Address = u.Address,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            })
            .AsNoTracking()
            .ToListAsync();

            return adminUsers;
        }

        public async Task<GetUserModel> GetAdmin(Guid id)
        {
            bool hasAccessAdmin = _user.IsInRole("Admin");
            if (!hasAccessAdmin)
            {
                throw new ForbiddenException("Not Allowed");
            }

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new NotFoundException("Not Found");
            }
            var roleNames = await _userManager.GetRolesAsync(user);

            return new GetUserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                MimeTypeImageData = user.MimeTypeImageData,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Street = user.Street,
                Address = user.Address,
                Roles = roleNames,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<GetUserModel> GetAdmin()
        {
            bool hasAccessAdmin = _user.IsInRole("Admin");
            if (!hasAccessAdmin)
            {
                throw new ForbiddenException("Not Allowed");
            }

            Guid userId = new Guid(_user.Identity.Name);
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                throw new NotFoundException("Not Found");
            }
            var roleNames = await _userManager.GetRolesAsync(user);

            GetUserModel userModel = new GetUserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Street = user.Street,
                Address = user.Address,
                Roles = roleNames,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return userModel;
        }


        public async Task<GetUserModel> PostAdmin(PostUserModel postUserModel, string ipAddress)
        {
            bool hasAccessAdmin = _user.IsInRole("Admin");
            if (!hasAccessAdmin)
            {
                throw new ForbiddenException("Not Allowed");
            }

            User user = new User();

            user.FirstName = postUserModel.FirstName;
            user.LastName = postUserModel.LastName;
            user.DateOfBirth = postUserModel.DateOfBirth;
            user.Email = postUserModel.Email;
            user.ImageData = postUserModel.ImageData;
            user.MimeTypeImageData = postUserModel.MimeTypeImageData;
            user.UserName = postUserModel.Email;
            user.Country = postUserModel.Country;
            user.Province = postUserModel.Province;
            user.City = postUserModel.City;
            user.PostalCode = postUserModel.PostalCode;
            user.Street = postUserModel.Street;
            user.Address = postUserModel.Address;

            var createResult = await _userManager.CreateAsync(user, postUserModel.Password);
            if (!createResult.Succeeded)
            {
                throw new Exception($"User creation failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
            if (!roleResult.Succeeded)
            {
                // Handle failure: possibly throw an exception or return an error response
                throw new Exception($"Failed to add user to role Admin: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }


            await _context.SaveChangesAsync();
            var roleNames = await _userManager.GetRolesAsync(user);
            return new GetUserModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                ImageData = user.ImageData,
                MimeTypeImageData = user.MimeTypeImageData,
                Country = user.Country,
                Province = user.Province,
                City = user.City,
                PostalCode = user.PostalCode,
                Street = user.Street,
                Address = user.Address,
                Roles = roleNames,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };
        }


    }
}
