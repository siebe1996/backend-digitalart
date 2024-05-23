using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Expositions
{
    public class GetExpositionExpandedModel : BaseExpositionModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public ICollection<Guid> CategoryIds { get; set; }

        [Required]
        public ICollection<Guid> RentalAgreementIds { get; set; }

        [Required]
        public ICollection<Guid> ProjectorIds { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
