using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Projectors
{
    public class PatchProjectorModel
    {
        public Guid? ExpositionId { get; set; }

        public bool? Available { get; set; }

        public string? Brand { get; set; }

        public string? Model { get; set; }

        public string? SerialNumber { get; set; }

        public string? Damages { get; set; }

        public string? Remarks { get; set; }
    }
}
