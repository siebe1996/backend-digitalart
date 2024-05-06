using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Projectors
{
    public class GetProjectorModel : BaseProjectorModel
    {
        [Required]
        public Guid Id { get; set; }

        public Guid? ExpositionId { get; set; }

        [Required]
        public bool Available { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
