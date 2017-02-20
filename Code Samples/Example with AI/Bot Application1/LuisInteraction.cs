using Bot_Application1.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Bot_Application1
{
    public class LuisInteraction
    {
        public IntentResponse GetIntentFromQuestionText(string inputText)
        {
            var luisUrl = string.Format("https://api.projectoxford.ai/luis/v2.0/apps/ad59b259-2c09-4f47-997a-35189b9517dc?subscription-key=c823d3e7562244218b7e801ea28d3cc1&verbose=true&q={0}", inputText);
            WebRequest aiRequest = WebRequest.Create(luisUrl);
            WebResponse aiResponse = aiRequest.GetResponse();
            return ConvertJsonToIntentResponse(aiResponse);
        }

        private IntentResponse ConvertJsonToIntentResponse(WebResponse queryResponse)
        {
            using (var reader = new StreamReader(queryResponse.GetResponseStream()))
            {
                JToken token = JObject.Parse(reader.ReadToEnd());
                var topScoringIntent = token.SelectToken("topScoringIntent");
                if (Convert.ToDouble(topScoringIntent.SelectToken("score")) > 0.6)
                {
                    var response = new IntentResponse()
                    {
                        IntentMatched = true,
                        Intent = topScoringIntent.SelectToken("intent").ToString()
                    };
                    response.Entities = new List<string>();
                    foreach (var entity in token.SelectTokens("entities"))
                    {
                        if (entity.Count() > 0)
                        {
                            foreach (var innerEntity in entity)
                            {
                                if (Convert.ToDouble(innerEntity.SelectToken("score")) > 0.6)
                                {
                                    response.Entities.Add(innerEntity.SelectToken("entity").ToString());
                                }
                            }
                        }
                    }
                    return response;
                }
                else
                {
                    return new IntentResponse() { IntentMatched = false };
                }
            }
        }
    }
}