using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    public class ExhibitorPlace
    {
        [Required]
        public Guid ExhibitorId { get; set; }
        [Required]
        public Guid PlaceId { get; set; }

        //navigation props
        public virtual Exhibitor Exhibitor { get; set; }
        public virtual Place Place { get; set; }
    }
}
