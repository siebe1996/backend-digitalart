using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Places
{
    public class PatchPlaceModel
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        [DataType(DataType.PostalCode)]
        public string? PostalCode { get; set; }

        public string? Province { get; set; }

        public string? Address { get; set; }

        public string? Street { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
