using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hospital.Controllers
{
    
    [Route("Doctor")]
    [ServiceFilter(typeof(UserLogFilter))]
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    public class DoctorController : Controller
    {
        private readonly ILogger _logger;
        private readonly IDistributedCache _distributedCache;

        public DoctorController(ILogger<HomeController> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }
        // UserContext context = new UserContext();
        HospitalContext context = new HospitalContext();
        [HttpGet]
        public IActionResult ListDoctors()
        {
            var json = _distributedCache.GetString("doctors");

            List<Doctor> doctors;

            if (json == null)
            {
                doctors = context.Doctors.ToList();
                var jsonSave = JsonConvert.SerializeObject(doctors);
                _distributedCache.SetString("doctors", jsonSave, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
                });
            }
            else
            {
                doctors = JsonConvert.DeserializeObject<List<Doctor>>(json);
            }
            _logger.LogInformation("listDoctor");
            return View(context.Doctors.ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Create")]
        public IActionResult CreateDoctor()
        {
            _logger.LogInformation("CreateDoctor");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Create")]
        public IActionResult CreateDoctor(Doctor doctor) 
        {
                context.Doctors.Add(doctor);
                context.SaveChanges();
                _logger.LogInformation(DateTime.Now.ToString() + ": new Doctor added: " + doctor.name);
                return RedirectToAction("ListDoctors", "Doctor");   
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult EditDoctor(int id)
        {
            _logger.LogInformation("EditDoctor");
            return View(context.Doctors.Find(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Edit/{id}")]
        public IActionResult EditDoctor(Doctor doctor)
        {
            context.Doctors.Update(doctor);
            context.SaveChanges();
            _logger.LogInformation(DateTime.Now.ToString() + ": Doctor edit: " + doctor.name);
            return RedirectToAction("ListDoctors", "Doctor");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            context.Doctors.Remove(context.Doctors.Find(id));
            context.SaveChanges();
            _logger.LogInformation(DateTime.Now.ToString() + ": Doctor delete: " + id);
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
            _logger.LogInformation(DateTime.Now.ToString() + ": Doctor update: " + doctor.id);
            return RedirectToAction("ListDoctors", "Doctors");
        }
        

    }
}
