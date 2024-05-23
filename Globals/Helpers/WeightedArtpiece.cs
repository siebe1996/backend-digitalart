using Globals.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Helpers
{
    public class WeightedArtpiece : IWeighted
    {
        public Guid Id { get; set; }
        public double Score { get; set; }
    }
}
