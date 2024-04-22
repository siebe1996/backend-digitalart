using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Roles
{
    public class BaseRoleModel
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Description { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
    }
}
