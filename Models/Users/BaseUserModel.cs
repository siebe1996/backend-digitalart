using Globals.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Users
{
    public class BaseUserModel
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        public string? Description { get; set; }

        /*[Required]
        [StringLength(50, MinimumLength = 2)]
        public string UserName { get; set; }*/

        /*[Required]
        [EnumDataType(typeof(Gender))]
        public Gender Gender { get; set; }*/

        /*[Required]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string PhoneNumber { get; set;}*/

        [Required]
        public string Country {  get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }

        [Required]
        public string Province { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
