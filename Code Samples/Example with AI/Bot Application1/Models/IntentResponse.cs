using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application1.Models
{
    public class IntentResponse
    {
        public bool IntentMatched { get; set; }
        public string Intent { get; set; }
        public List<string> Entities { get; set; }
    }
}