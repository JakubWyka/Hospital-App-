using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Models
{
    public enum AppointmentStatusType
    {
        [Display(Name = "Open")] Open,
        [Display(Name = "Reserved")] Reserved
    }
    public class Appointment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [NotMapped]
        public int doctorId { get; set; }
        public Doctor doctor { get; set; }
        [NotMapped]
        public int? patientId { get; set; }
        public Patient patient { get; set; }

        [DataType(DataType.Date)]
        public DateTime date { get; set; }

        public AppointmentStatusType status { get; set; }

    }
}
