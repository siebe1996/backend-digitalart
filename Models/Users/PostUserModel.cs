using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Users
{
    public class PostUserModel : BaseUserModel
    {
        //public string? ImageData { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public Guid Role {  get; set; }
    }
}
