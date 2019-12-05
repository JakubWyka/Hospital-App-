using Hospital.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Hospital.Models
{
    [MiddlewareFilter(typeof(LocalizationPipeline))]
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
