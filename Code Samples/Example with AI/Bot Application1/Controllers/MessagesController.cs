using Bot_Application1.Models;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Bot_Application1
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                //send user phrase to LUIS to get any intent
                var luis = new LuisInteraction();
                var intent = luis.GetIntentFromQuestionText(activity.Text);

                if (intent.IntentMatched == true && intent.Intent == "NextSession")
                {
                    //user wants to know about next session
                    var sessions = new SessionListAPI();
                    string responseText = ConvertIntentToResponseText(sessions, intent);
                    Activity reply = activity.CreateReply(responseText);
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
                else
                {
                    //user wants something else.
                    Activity unknownReply = activity.CreateReply("Sorry, I don't know how to help you");
                    await connector.Conversations.ReplyToActivityAsync(unknownReply);
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private string ConvertIntentToResponseText(SessionListAPI sessionsRepro, IntentResponse intent)
        {
            var reply = new StringBuilder();
            if (intent.Entities.Count == 0)
            {
                //no entities, make API call for all tracks
                var sessions = sessionsRepro.GetNextSession();
                if (sessions.Count() == 0)
                {
                    reply.Append($"There aren't any more sessions to show you");
                }
                else
                {
                    foreach (var session in sessions)
                    {
                        reply.Append($"The next {session.Track} track session is { session.Title} by { session.Author}. It starts at { session.StartTime.ToShortTimeString()} and lasts for { session.LengthMins} mins in { session.Location}\n\n");
                    }
                }
            }

            //otherwise, need to consider each entity and build up response from the getnext API call from each one
            foreach (var entity in intent.Entities)
            {
                var session = sessionsRepro.GetNextSession(entity);
                if (session != null)
                {
                    reply.Append($"The next session in the {session.Track} track is {session.Title} by {session.Author}. It starts at {session.StartTime.ToShortTimeString()} and lasts for {session.LengthMins} mins in {session.Location}\n\n");
                }
                else
                {
                    reply.Append($"There are no more {entity} sessions today. :(");
                }
            }

            return reply.ToString();
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened               
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}