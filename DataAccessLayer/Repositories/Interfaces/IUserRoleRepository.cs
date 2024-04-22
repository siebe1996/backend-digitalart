using Models.UsersRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IUserRoleRepository
    {
        Task<List<GetUserRoleModel>> GetUserRoles();
        Task<List<GetUserRoleModel>> GetUserRolesByUserId(Guid id);
        Task<List<GetUserRoleModel>> GetUserRolesByRoleId(Guid id);
        Task<GetUserRoleModel> GetUserRoleByIds(Guid userId, Guid roleId);
        Task<GetUserRoleModel> AddUserToRole(PostUserRoleModel postUserRoleModel);
    }
}
