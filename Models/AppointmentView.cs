using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public class AppointmentView
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int id { get; set; }
        [Display(Name = "Select date")]
        [DataType(DataType.Date)]
        public DateTime selectedDate { get; set; }

        [Display(Name = "Select doctor")]
        public Doctor selectedDoctor { get; set; }

    }
}
