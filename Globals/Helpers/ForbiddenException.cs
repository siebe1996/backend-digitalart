using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Helpers
{
    public class ForbiddenException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public ForbiddenException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.Forbidden;
        }
    }
}
