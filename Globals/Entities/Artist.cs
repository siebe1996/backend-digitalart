using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Entities
{
    [Table("Artists")]
    public class Artist : User
    {
        //toDo make field artist name

        [Required]
        public string Description { get; set; }

        public virtual ICollection<Artpiece> Artpieces { get; set; }
    }
}
