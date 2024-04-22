using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Artpieces
{
    public class PostArtpieceModel : BaseArtpieceModel
    {
        [Required]
        public List<Guid> CategoryIds { get; set; }
    }
}
