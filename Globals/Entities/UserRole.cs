using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class UserRole : IdentityUserRole<Guid>
    {
        //navigation props
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
