using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Expositions
{
    public class PutExpositionModel : BaseExpositionModel
    {
        [Required]
        public bool Active { get; set; }

        [Required]
        public List<Guid> ProjectorIds { get; set; }

        [Required]
        public List<Guid> CategoryIds { get; set; }
    }
}
