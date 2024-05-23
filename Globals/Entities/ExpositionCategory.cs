using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class ExpositionCategory
    {
        [Required]
        public Guid ExpositionId { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Exposition Exposition { get; set; }
    }
}
