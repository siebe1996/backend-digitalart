﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Expositions
{
    public class BaseExpositionModel
    {
        [Required]
        public string Name { get; set; }
    }
}
