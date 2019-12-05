using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860


namespace Hospital.Controllers
{
    [Authorize(Roles = "Admin, Doctor")]
    [Route("Patient")]
    [ServiceFilter(typeof(UserLogFilter))]
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    public class PatientController : Controller
    {
        private IDistributedCache _distributedCache;
        private readonly ILogger _logger;

        public PatientController(ILogger<HomeController> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }
        //UserContext context = new UserContext();
        HospitalContext context = new HospitalContext();
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


            _logger.LogInformation("ListPatients");
            return View(patients);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult CreatePatient()
        {
            _logger.LogInformation("CreatePatient");
            return View();
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult CreatePatient(Patient patient) 
        {
             context.Patients.Add(patient);
             context.SaveChanges();
            _logger.LogInformation(DateTime.Now.ToString() + ": new Patient added: " + patient.name);
            return RedirectToAction("ListPatients", "Patient");   
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult EditPatient(int id)
        {
            var patient = context.Patients.Find(id);
            _logger.LogInformation("EditPatients");
            return View(patient);
        }

        [HttpPost]
        [Route("Edit/{id}")]
        public IActionResult EditPatient(Patient patient)
        {
            context.Patients.Update(patient);
            context.SaveChanges();
            _logger.LogInformation(DateTime.Now.ToString() + ": Patient edit: " + patient.name);
            return RedirectToAction("ListPatients", "Patient");
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            context.Patients.Remove(context.Patients.Find(id));
            context.SaveChanges();
            _logger.LogInformation(DateTime.Now.ToString() + ": Patient edit: id:" + id );
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
            _logger.LogInformation(DateTime.Now.ToString() + ": Patient update: " + patient.name);
            context.SaveChanges();
            return RedirectToAction("ListPatients", "Patients");
        }
        

    }
}
