using Globals.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class Like : ITrackable
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ArtpieceId { get; set; }

        [Required]
        public bool Liked { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        //navigation props
        public virtual User User { get; set; }
        public virtual Artpiece Artpiece { get; set; }
    }
}
