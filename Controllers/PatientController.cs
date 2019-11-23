using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hospital.Controllers
{
    [Route("Patient")]
    public class PatientController : Controller
    {
        HospitalContext context = new HospitalContext();

        [HttpGet]
        public IActionResult ListPatients()
        {
            return View(context.Patients.ToList());
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult CreatePatient()
        {
            return View();
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult CreatePatient(Patient patient) 
        {
                context.Patients.Add(patient);
                context.SaveChanges();
                return RedirectToAction("ListPatients", "Patient");   
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult EditPatient(int id)
        {
            var patient = context.Patients.Find(id);
            return View(patient);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        public IActionResult EditPatient(Patient patient)
        {
            context.Patients.Update(patient);
            context.SaveChanges();
            return RedirectToAction("ListPatients", "Patient");
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            context.Patients.Remove(context.Patients.Find(id));
            context.SaveChanges();
            return RedirectToAction("ListPatients", "Patient");
        }

        /*
        [HttpGet("/{id}")]
        public IActionResult GetPatient(int id) {
        
        }
        */
        [HttpPut("/{id}")]
        public IActionResult UpdatePatient(Patient patient) 
        {
            Patient p = context.Patients.Where(s => s.id == patient.id).First();
            p.name = patient.name;
            p.birthDate = patient.birthDate;
            p.userId = patient.userId;
    
            context.SaveChanges();
            return RedirectToAction("ListPatients", "Patients");
        }
        

    }
}
