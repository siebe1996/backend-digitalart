using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class ArtpieceCategory
    {
        [Required]
        public Guid CategoryId { get; set; }
        [Required]
        public Guid ArtpieceId { get; set; }

        //navigation props
        public virtual Category Category { get; set; }
        public virtual Artpiece Artpiece { get; set; }
    }
}
