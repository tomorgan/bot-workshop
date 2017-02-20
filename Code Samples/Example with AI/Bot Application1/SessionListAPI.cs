using Bot_Application1.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Bot_Application1
{
    public class SessionListAPI
    {
        private string nextSessionURL = "http://nordevconwebapi.azurewebsites.net/api/onnext?day=1&time=0830";
        private string todaySessionsURL = "http://nordevconwebapi.azurewebsites.net/api/today?day=1&time=0830";

        public Session GetNextSession(string track)
        {
            var url = nextSessionURL;
            if (track != null) url = url + "&track=" + track; //CHANGE if removing day/time
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var json = reader.ReadToEnd();
                var session = JsonConvert.DeserializeObject<Session>(json);
                return session;
            }
        }

        public IEnumerable<Session> GetNextSession()
        {
            var url = nextSessionURL;
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var json = reader.ReadToEnd();
                var sessions = JsonConvert.DeserializeObject<IEnumerable<Session>> (json);
                return sessions;
            }
        }

        public IEnumerable<Session> GetTodaysSession(string track)
        {
           var url = todaySessionsURL;
            if (track != null) url = url + "&track=" + track; //CHANGE if removing day/time
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var json = reader.ReadToEnd();
                var sessions = JsonConvert.DeserializeObject<IEnumerable<Session>>(json);
                return sessions;
            }
        }

       


    }
}