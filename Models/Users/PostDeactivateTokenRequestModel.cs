using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Users
{
    public class PostDeactivateTokenRequestModel
    {
        //[Required]
        public string Token { get; set; }
    }
}
