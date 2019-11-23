using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hospital.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hospital.Controllers
{
    [Route("Appointment")]
    public class AppointmentController : Controller
    {
        UserContext context = new UserContext();

        [HttpGet]
        public IActionResult ListAppointments()
        {
            ViewBag.Doctors = new SelectList(context.Doctors, "id", "name");
            return View();
        }

        [HttpGet]
        [Route("Week/{date}/{doctorId}")]
        public IActionResult WeekAppointments(DateTime date, int doctorId)
        {
            int diff = ((7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7);
            date = date.AddDays((-1) * diff).Date;
            date = date - date.TimeOfDay;
            List<Appointment> appointments = context.Appointments.Where(a => a.date >= date && a.date < date.AddDays(7)).Where(a => a.doctor.id == doctorId).ToList();
            

            return PartialView(appointments);
        }

        [Authorize(Roles = "Admin, Doctor")]
        [HttpGet]
        [Route("Create")]
        public IActionResult CreateAppointment()
        {
            ViewBag.Patients = new SelectList(context.Patients, "id", "name");
            ViewBag.Doctors = new SelectList(context.Doctors, "id", "name");
            return View();
        }

        [Authorize(Roles = "Admin, Doctor")]
        [HttpPost]
        [Route("Create")]
        public IActionResult CreateAppointment(Appointment appointment) 
        {
            appointment.doctor = context.Doctors.Find(appointment.doctorId);
            if (appointment.patientId != null) appointment.patient = context.Patients.Find(appointment.patientId);
            context.Appointments.Add(appointment);
                context.SaveChanges();
                return RedirectToAction("ListAppointments", "Appointment");   
        }

        [Authorize(Roles = "Admin, Doctor")]
        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult EditAppointment(int id)
        {
            return View(context.Appointments.Find(id));
        }

        [Authorize(Roles = "Admin, Doctor")]
        [HttpPost]
        [Route("Edit/{id}")]
        public IActionResult EditAppointment(Appointment appointment)
        {
            context.Appointments.Update(appointment);
            context.SaveChanges();
            return RedirectToAction("ListAppointments", "Appointment");
        }

        [Authorize(Roles = "Patient, Admin")]
        [HttpGet]
        [Route("Reserve/{id}")]
        public IActionResult ReserveAppointment(int id)
        {

            return View(context.Appointments.Find(id));
        }

        [Authorize(Roles = "Patient, Admin")]
        [HttpPost]
        [Route("Reserve/{id}")]
        public IActionResult ReserveAppointment(Appointment appointment)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            string userIdValue = "null";
            var userIdClaim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            userIdValue = userIdClaim.Value;
            // Check if patient to be done

            appointment.patient = context.Patients.Where(s => s.userId == userIdValue).FirstOrDefault();




            context.Appointments.Update(appointment);
            appointment.status = AppointmentStatusType.Reserved;
            context.SaveChanges();
            return RedirectToAction("ListAppointments", "Appointment");
        }

        [Authorize("Admin, Doctor")]
        [HttpPost]
        [Route("Delete")]
        public IActionResult Delete(int id)
        {
            context.Appointments.Remove(context.Appointments.Find(id));
            context.SaveChanges();
            return RedirectToAction("ListAppointments", "Appointment");
        }

        /*
        [HttpGet("/{id}")]
        public IActionResult GetPatient(int id) {
        
        }
        */
        [Authorize("Admin, Doctor")]
        [HttpPut("/{id}")]
        public IActionResult UpdateAppointment(Appointment appointment) 
        {
            Appointment p = context.Appointments.Where(s => s.id == appointment.id).First();
            p.date = appointment.date;
            p.doctor = appointment.doctor;
            p.patient = appointment.patient;
            p.status = appointment.status;
    
            context.SaveChanges();
            return RedirectToAction("ListAppointments", "Appointment");
        }
        

    }
}
