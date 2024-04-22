using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class ExpositionArtpiece
    {
        [Required]
        public Guid ExpositionId { get; set; }
        [Required]
        public Guid ArtpieceId { get; set; }

        //navigation props
        public virtual Exposition Exposition { get; set; }
        public virtual Artpiece Artpiece { get; set; }
    }
}
