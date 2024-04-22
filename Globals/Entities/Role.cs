using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class Role : IdentityRole<Guid>
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Description { get; set; }

        //navigation props
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
