using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Users
{
    public class PutUserModel
    {
        /*public string? UserName { get; set; }*/
        public DateTime? DateOfBirth { get; set; }
        public byte[]? ImageData { get; set; }

        public string? MimeTypeImageData { get; set; }
    }
}
