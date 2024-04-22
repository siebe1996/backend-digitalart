using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Models.Users
{
    public class ResetPasswordModel
    {
        public string Email { get; set; }

        //this is optional cause i dont use email to reset password and just do it in app
        public string? Token { get; set; }
        public string NewPassword { get; set; }
    }
}
