﻿using System;
using System.Collections.Generic;

#nullable disable

namespace SE1608_Group1_A3.Models
{
    public partial class Film
    {
        public Film()
        {
            Shows = new HashSet<Show>();
        }

        public int FilmId { get; set; }
        public int GenreId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string CountryCode { get; set; }

        public virtual Genre Genre { get; set; }
        public virtual ICollection<Show> Shows { get; set; }
    }
}
