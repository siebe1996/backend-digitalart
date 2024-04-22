using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class Artpiece
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ArtistId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ImageData { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public virtual Artist Artist { get; set; }
        public virtual ICollection<ExpositionArtpiece> ExpositionArtpieces { get; set; }
        public virtual ICollection<ArtpieceCategory> ArtpieceCategories { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}
