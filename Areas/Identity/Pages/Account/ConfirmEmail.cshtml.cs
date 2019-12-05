using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Hospital.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Tiny.RestClient;

namespace Hospital.Areas.Identity.Pages.Account
{
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            StatusMessage = result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.";

            

            if(result.Succeeded)
            {
                

                //new Controllers.PatientController().CreatePatient(pat)
                /*String CreatePatient = Url.Action(
                    "Create", "Patient", null, "https"
                    );

                //var client = new TinyRestClient(new HttpClient(), CreatePatient);

                //var response = await client.PostRequest("", patient).ExecuteAsync<bool>();

                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(string.Format(CreatePatient));
                webReq.Method = "POST";

                //DataContractJsonSerializer ser = new DataContractJsonSerializer(patient.GetType());
                StreamWriter writer = new StreamWriter(webReq.GetRequestStream());
                // string yourdata = jss.Deserialize<UserInputParameters>(stdObj);
                string yourdata = JsonConvert.SerializeObject(patient);
                writer.Write(yourdata);
                writer.Close();





                HttpWebResponse webResponse = (HttpWebResponse)webReq.GetResponse();
                int ads = 4;*/
            }
            return Page();
        }
    }
}
