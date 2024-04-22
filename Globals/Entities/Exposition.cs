using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class Exposition
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<Projector> Projectors { get; set; }
        public virtual ICollection<ExpositionArtpiece> ExpositionArtpieces { get; set; }
    }
}
