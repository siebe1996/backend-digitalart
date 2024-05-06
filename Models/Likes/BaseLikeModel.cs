using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Likes
{
    public class BaseLikeModel
    {
        [Required]
        public Guid ArtpieceId { get; set; }

        [Required]
        public bool Liked { get; set; }
    }
}
