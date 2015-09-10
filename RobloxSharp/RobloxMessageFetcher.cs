using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobloxSharp
{
    public class RobloxMessageFetcher
    {
        /// <summary>
        /// Fetches 20 messages from the pageNumber specified, requires authentication
        /// </summary>
        /// <param name="pageNumber">Page number to get</param>
        /// <param name="cookies">Authentication cookie string</param>
        /// <returns>MessageCollection object containing all messages on the pageNumber</returns>
        public MessageCollection getNewMessages(String pageNumber, String cookies)
        {
            String page = RobloxUtils.readPage("http://www.roblox.com/messages/api/get-messages?messageTab=0&pageNumber=" + pageNumber + "&pageSize=20", cookies);
            return JsonConvert.DeserializeObject<MessageCollection>(page);
        }
    }
    public class MessageSender
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int BuildersClubStatus { get; set; }
    }

    public class MessageRecipient
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int BuildersClubStatus { get; set; }
    }

    public class SenderThumbnail
    {
        public bool Final { get; set; }
        public string Url { get; set; }
        public object RetryUrl { get; set; }
    }

    public class RecipientThumbnail
    {
        public bool Final { get; set; }
        public string Url { get; set; }
        public object RetryUrl { get; set; }
    }
    public class Message
    {
        public int Id { get; set; }
        public MessageSender Sender { get; set; }
        public MessageRecipient Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public SenderThumbnail SenderThumbnail { get; set; }
        public RecipientThumbnail RecipientThumbnail { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public bool IsRead { get; set; }
        public bool IsSystemMessage { get; set; }
        public bool IsReportAbuseDisplayed { get; set; }
    }
    public class MessageCollection
    {
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public List<Message> Collection { get; set; }
        public int TotalCollectionSize { get; set; }
    }
}
