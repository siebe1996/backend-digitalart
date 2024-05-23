using Models.Artists;
using Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<PostAuthenticateResponseModel> Authenticate(PostAuthenticateRequestModel postAuthenticateRequestModel, string ipAddress);
        Task<PostAuthenticateResponseModel> RenewToken(string token, string ipAddress);
        Task DeactivateToken(string token, string ipAddress);
        Task<List<GetUserModel>> GetUsers();
        Task<GetUserModel> GetUser(Guid id);
        Task<GetUserModel> GetUser();
        Task<GetUserModel> PostUser(PostUserModel postUserModel, string ipAddress);
        Task<GetUserModel> PutUser(Guid id, PutUserModel putUserModel);
        Task<bool> ResetPassword(ResetPasswordModel resetPasswordModel);

        Task<List<GetUserModel>> GetAdmins();
        Task<GetUserModel> GetAdmin(Guid id);
        Task<GetUserModel> GetAdmin();
        Task<GetUserModel> PostAdmin(PostUserModel postUserModel, string ipAddress);
    }
}
