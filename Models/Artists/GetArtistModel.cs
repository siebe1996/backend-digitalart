using Models.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Artists
{
    public class GetArtistModel : BaseArtistModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public ICollection<string> Roles { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
