using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Hospital.Controllers
{
    [Route("Appointment")]
    public class AppointmentController : Controller
    {
        HospitalContext context = new HospitalContext();

        [HttpGet]
        public IActionResult ListAppointments()
        {
            return View();
            //return View(context.Appointments.ToList());
        }

        [HttpGet]
        [Route("Week/{date}")]
        public IActionResult WeekAppointments(DateTime date)
        {
            int diff = ((7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7);
            date = date.AddDays((-1) * diff).Date;
            date = date - date.TimeOfDay;
            List<Appointment> appointments = context.Appointments.Where(a => a.date >= date && a.date < date.AddDays(7)).ToList();
            

            return PartialView(appointments);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult CreateAppointment()
        {
            ViewBag.Patients = new SelectList(context.Patients, "id", "name");
            
            ViewBag.Doctors = new SelectList(context.Doctors, "id", "name");
            return View();
        }

        [HttpPost]
        [Route("Create")]
        public IActionResult CreateAppointment(Appointment appointment) 
        {
                context.Appointments.Add(appointment);
                context.SaveChanges();
                return RedirectToAction("ListAppointments", "Appointment");   
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult EditAppointment(int id)
        {
            return View(context.Appointments.Find(id));
        }

        [HttpPost]
        [Route("Edit/{id}")]
        public IActionResult EditAppointment(Appointment appointment)
        {
            context.Appointments.Update(appointment);
            context.SaveChanges();
            return RedirectToAction("ListAppointments", "Appointment");
        }

        [HttpGet]
        [Route("Reserve/{id}")]
        public IActionResult ReserveAppointment(int id)
        {
            return View(context.Appointments.Find(id));
        }

        [HttpPost]
        [Route("Reserve/{id}")]
        public IActionResult ReserveAppointment(Appointment appointment)
        {
            context.Appointments.Update(appointment);
            appointment.status = AppointmentStatusType.Reserved;
            context.SaveChanges();
            return RedirectToAction("ListAppointments", "Appointment");
        }

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
