using Globals.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class Projector : ITrackable
    {
        [Required]
        public Guid Id { get; set; }

        public Guid? ExpositionId { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string SerialNumber { get; set; }

        public string? Damages { get; set; }

        public string? Remarks { get; set; }

        [Required]
        public bool Available { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public virtual Exposition Exposition { get; set; }
        public virtual ICollection<RentalAgreement> RentalAgreements { get; set; }
    }
}
