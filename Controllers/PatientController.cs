using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


namespace Hospital.Controllers
{
    [Authorize(Roles = "Admin, Doctor")]
    [Route("Patient")]
    public class PatientController : Controller
    {
        HostpitalContext context = new HostpitalContext();

        private IDistributedCache _distributedCache;

        public PatientController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public IActionResult ListPatients()
        {
            var json = _distributedCache.GetString("patients");

            List<Patient> patients;

            if(json == null)
            {
                patients = context.Patients.ToList();
                var jsonSave = JsonConvert.SerializeObject(patients);
                _distributedCache.SetString("patients", jsonSave, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
                });
            }
            else
            {
                patients = JsonConvert.DeserializeObject<List<Patient>>(json);
            }


            
            return View(patients);
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
