using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RentalAgreements
{
    public class BaseRentalAgreementModel
    {
        [Required]
        public Guid PlaceId { get; set; }

        [Required]
        public Guid ProjectorId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
