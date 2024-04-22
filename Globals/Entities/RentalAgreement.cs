using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class RentalAgreement
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid PlaceId { get; set; }

        [Required]
        public Guid ProjectorId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public virtual Projector Projector { get; set; }
        public virtual Place Place { get; set; }
    }
}
