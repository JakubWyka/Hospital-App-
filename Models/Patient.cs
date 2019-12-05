using Hospital.Controllers;
using Microsoft.AspNetCore.Identity;
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
    public class Patient
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Display(Name = "Full name")]
        public string name { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Birth date")]
        public DateTime birthDate { get; set; }
        public string userId { get; set; }

     
    }
}
