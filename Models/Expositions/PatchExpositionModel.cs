using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Expositions
{
    public class PatchExpositionModel
    {
        [Required]
        public bool Active { get; set; }
    }
}
