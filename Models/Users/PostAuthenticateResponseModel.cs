using Globals.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models.Users
{
    public class PostAuthenticateResponseModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        /*[Required]
        [StringLength(50, MinimumLength = 2)]
        public string UserName { get; set; }*/

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string JwtToken { get; set; }

        public byte[]? ImageData { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        /*[Required]
        public double Score { get; set; }*/

        public ICollection<string> Roles { get; set; }

        [JsonIgnore]
        public string RefreshToken { get; set; }

        /*[Required]
        public Gender Gender { get; set; }*/

        /*[Required]
        public string PhoneNumber { get; set; }*/

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string Province { get; set; }

        [Required]
        public string Address { get; set; }

        public string? Description { get; set; }

        /*public string? StripeAccountId { get; set; }*/

        /*public string? StripeCostumerId { get; set; }*/

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
