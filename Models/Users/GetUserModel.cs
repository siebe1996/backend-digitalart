using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Users
{
    public class GetUserModel : BaseUserModel
    {
        [Required]
        public Guid Id { get; set; }

        public List<string> Roles { get; set; }

        public byte[]? ImageData { get; set; }

        /*[Required]
        public double Score { get; set; }*/

        /*public string? StripeAccountId { get; set; }*/

        /*public string? StripeCostumerId { get; set; }*/

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
