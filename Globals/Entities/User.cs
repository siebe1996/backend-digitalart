using Globals.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class User : IdentityUser<Guid>
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        /*[Required]
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }*/

        [Required]
        public string Country { get; set; }

        [Required]
        public string Province { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string Address { get; set; }

        public byte[]? ImageData { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateOfBirth { get; set; }

        /*[Required]
        public double Score { get; set; }

        public string? StripeAccountId { get; set; }

        public string? StripeCostumerId { get; set; }*/

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        //navigation props
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<Guid>> Tokens { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}
