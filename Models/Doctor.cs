using Hospital.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hospital.Models
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    public enum SpecializationType
    {
        [Display(Name = "Cardiologist")] Cardiologist,
        [Display(Name = "Anesthesiologist")] Anesthesiologist,
        [Display(Name = "Dermatologist")] Dermatologist,
        [Display(Name = "Neurologist")] Neurologist
    }
    public class Doctor
    {
       [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       public int id { get; set; }

        [Display(Name = "Full name")]
        public string name { get; set; }

        [Display(Name = "Specialization")]
        public SpecializationType specializationType { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Job start")]
        public DateTime jobSeniority { get; set; }
        public string userId { get; set; }


    }
}
