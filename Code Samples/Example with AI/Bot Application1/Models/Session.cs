using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application1.Models
{
    public class Session
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime StartTime { get; set; }
        public int LengthMins { get; set; }
        public string Location { get; set; }
        public string Track { get; set; }

    }
}