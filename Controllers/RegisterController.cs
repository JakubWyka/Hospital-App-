using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hospital.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hospital.Controllers
{
    [Route("Register")]
    public class RegisterController : Controller
    {
        [HttpGet]
        [Route("RegisterType/{registerType}")]
        public IActionResult RegisterType(string registerType)
        {
            if (registerType == "Doctor")
            {
                return PartialView("DoctorRegister");
            }
            else
            {
                return PartialView("PatientRegister");
            }
        }

    }
}
