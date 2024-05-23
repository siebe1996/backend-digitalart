using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Interfaces
{
    public interface IWeighted
    {
        Guid Id { get; set; }
        double Score { get; set; }
    }
}
