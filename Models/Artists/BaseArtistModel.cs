using Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Artists
{
    public class BaseArtistModel : BaseUserModel
    {
        public string Description { get; set; }
    }
}
