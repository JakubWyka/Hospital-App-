using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class Patient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Display(Name = "Full name")]
        public string name { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Birth date")]
        public int birthDate { get; set; }

     
    }
}
