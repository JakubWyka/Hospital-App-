using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
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
        private IConverter _converter;

        public AppointmentController(IConverter converter)
        {
            _converter = converter;
        }
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


            public string GetHTMLString()
            {
            var appointments = context.Appointments;
            var pa = context.Patients.ToList();
            var doc = context.Doctors.ToList();
            var sb = new System.Text.StringBuilder();
                
                sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <div class='header'><h1>This is the generated PDF report!!!</h1></div>
                                <table align='center'>
                                    <tr>
                                        <th>Doctor</th>
                                        <th>Patient</th>
                                        <th>Date</th>
                                        
                                    </tr>");

                foreach (var emp in appointments)
                {
                int? idd = emp.doctorId;
                int? idp = emp.patientId;
                // string named = doc.Find(x => x.id == idd).name;
                string named = emp.doctor.name;
                //string namep = pa.Find(x => x.id == idp).name;
                string namep = emp.patient.name;
                string date = emp.date.ToString();
                    sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                
                                  </tr>", named,namep, date);
                }

                sb.Append(@"
                                </table>
                            </body>
                        </html>");

                return sb.ToString();
            }
        [Route("PDF")]
        public IActionResult CreatePDF(Appointment appointment)
        {

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
                Out = "Employee_Report.pdf"
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = GetHTMLString(),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };


            var file = _converter.Convert(pdf);
            // return File(Path.Combine(Directory.GetCurrentDirectory(), "Employee_Report.pdf"), "application/pdf");
            var fileStream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "Employee_Report.pdf"),
                                      FileMode.Open,
                                      FileAccess.Read
                                    );
            var fsResult = new FileStreamResult(fileStream, "application/pdf");
            return fsResult;

        }
    }
    }


