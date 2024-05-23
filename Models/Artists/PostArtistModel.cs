using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Artists
{
    public class PostArtistModel : BaseArtistModel
    {
        [Required]
        public string Password { get; set; }
    }
}
