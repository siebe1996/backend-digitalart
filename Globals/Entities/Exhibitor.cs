using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    [Table("Exhibitors")]
    public class Exhibitor : User
    {
        public virtual ICollection<ExhibitorPlace> ExhibitorPlaces { get; set; }
    }
}
