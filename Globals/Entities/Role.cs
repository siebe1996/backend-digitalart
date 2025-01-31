﻿using Globals.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class Role : IdentityRole<Guid>, ITrackable
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        //navigation props
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
