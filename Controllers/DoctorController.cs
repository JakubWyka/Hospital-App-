using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hospital.Controllers
{
    [Route("Doctor")]
    public class DoctorController : Controller
    {
        HospitalContext context = new HospitalContext();

        [HttpGet]
        public IActionResult ListDoctors()
        {
            return View(context.Doctors.ToList());
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult CreateDoctor()
        {
            return View();
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult CreateDoctor(Doctor doctor) 
        {
                context.Doctors.Add(doctor);
                context.SaveChanges();
                return RedirectToAction("ListDoctors", "Doctor");   
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult EditDoctor(int id)
        {
            return View(context.Doctors.Find(id));
        }

        [HttpPost]
        [Route("Edit/{id}")]
        public IActionResult EditDoctor(Doctor doctor)
        {
            context.Doctors.Update(doctor);
            context.SaveChanges();
            return RedirectToAction("ListDoctors", "Doctor");
        }

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
        [HttpPut("/{id}")]
        public IActionResult UpdateDoctor(Doctor doctor) 
        {
            Doctor p = context.Doctors.Where(s => s.id == doctor.id).First();
            p.name = doctor.name;
            p.specializationType = doctor.specializationType;
            p.jobSeniority = doctor.jobSeniority;
    
            context.SaveChanges();
            return RedirectToAction("ListDoctors", "Doctors");
        }
        

    }
}
