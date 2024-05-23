using Models.Places;
using Models.Projectors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RentalAgreements
{
    public class GetRentalAgreementExpandedModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public GetPlaceReducedModel Place { get; set; }

        [Required]
        public GetProjectorModel Projector { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
