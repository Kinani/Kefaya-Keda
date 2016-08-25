using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KefayaKeda.Models
{
    public class KKProfile
    {
        [Key]
        public int KKProfileId { get; set; }
        public string name { get; set; }
        public string WhatAction { get; set; }
        public TimeSpan TimeAllowance { get; set; }
        public TimeSpan TimeEllapsed { get; set; }
        public DateTime LastEdit { get; set; }
        public DateTime SessionStartTime { get; set; }
        public DateTime DateCreated { get; private set; }

        KKProfile()
        {
            DateCreated = DateTime.Now;
        }

    }
}
