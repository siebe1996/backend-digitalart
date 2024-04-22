using Globals.Entities;
using Models.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Artpieces
{
    public class GetArtpieceModel : BaseArtpieceModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid ArtistId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public List<string>? Expositions { get; set; }

        [Required]
        public List<GetCategoryModel> Categories { get; set; }
    }
}
