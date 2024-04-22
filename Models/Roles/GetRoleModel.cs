using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Roles
{
    public class GetRoleModel : BaseRoleModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}
