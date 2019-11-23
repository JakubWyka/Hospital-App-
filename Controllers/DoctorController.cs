using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hospital.Controllers
{
    
    [Route("Doctor")]
    public class DoctorController : Controller
    {
        UserContext context = new UserContext();

        [HttpGet]
        public IActionResult ListDoctors()
        {
            return View(context.Doctors.ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Create")]
        public IActionResult CreateDoctor()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Create")]
        public IActionResult CreateDoctor(Doctor doctor) 
        {
                context.Doctors.Add(doctor);
                context.SaveChanges();
                return RedirectToAction("ListDoctors", "Doctor");   
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult EditDoctor(int id)
        {
            return View(context.Doctors.Find(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Edit/{id}")]
        public IActionResult EditDoctor(Doctor doctor)
        {
            context.Doctors.Update(doctor);
            context.SaveChanges();
            return RedirectToAction("ListDoctors", "Doctor");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            context.Doctors.Remove(context.Doctors.Find(id));
            context.SaveChanges();
            return RedirectToAction("ListDoctors", "Doctor");
        }

        /*
        [HttpGet("/{id}")]
        public IActionResult GetPatient(int id) {
        
        }
        */
        [Authorize(Roles = "Admin")]
        [HttpPut("/{id}")]
        public IActionResult UpdateDoctor(Doctor doctor) 
        {
            Doctor p = context.Doctors.Where(s => s.id == doctor.id).First();
            p.name = doctor.name;
            p.specializationType = doctor.specializationType;
            p.jobSeniority = doctor.jobSeniority;
            p.userId = doctor.userId;
    
            context.SaveChanges();
            return RedirectToAction("ListDoctors", "Doctors");
        }
        

    }
}
