using Models.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<GetRoleModel>> GetRoles();
        Task<List<GetRoleModel>> GetAllRoles();
        Task<GetRoleModel> GetRole(Guid id);
        Task<GetRoleModel> GetRoleExhibitor();
        Task<GetRoleModel> PostRole(PostRoleModel postRoleModel);
    }
}
