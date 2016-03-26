using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RobloxSharp
{
    public class RobloxMessageHandler
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
        /// <summary>
        /// Sends a message with the supplied subject and content an ID
        /// </summary>
        /// <param name="receiverId">ID of the receiver</param>
        /// <param name="subject">Message subject</param>
        /// <param name="content">Message body</param>
        /// <param name="cookies">Authentication cookie string</param>
        /// <returns>Returns the success of the message sending</returns>
        public bool sendMessage(int receiverId, String subject, String content, String cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://m.roblox.com/messages/sendmessagework");

            request.UserAgent = RobloxUtils.UserAgent;
            request.Accept = "text/html, */*; q=0.01";
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
            request.Referer = "http://m.roblox.com/messages/sendmessage?Id=" + receiverId;
            request.Headers.Set(HttpRequestHeader.Cookie, cookies);
            request.KeepAlive = true;
            request.Headers.Set(HttpRequestHeader.Pragma, "no-cache");
            request.Headers.Set(HttpRequestHeader.CacheControl, "no-cache");

            request.Method = "POST";
            request.ServicePoint.Expect100Continue = false;

            string body = @"__RequestVerificationToken=" + getRequestVerificationToken(receiverId, cookies) + "&RecipientId=" + receiverId + "&Subject=" + subject + "&Body=" + content + "";

            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
            request.ContentLength = postBytes.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(postBytes, 0, postBytes.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = RobloxUtils.decodeStream(response);
            using (BufferedStream receiveStream = new BufferedStream(responseStream))
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return readStream.ReadToEnd().Contains("sent");
                }
            }
        }
        /// <summary>
        /// Gets an RVT for sending a message to the supplied receiverID
        /// </summary>
        /// <param name="receiverId">ID of the receiver</param>
        /// <param name="cookies">Authentication cookie string</param>
        /// <returns>Functional RVT</returns>
        public String getRequestVerificationToken(int receiverID,String cookies)
        {
            String response = RobloxUtils.readPage("http://m.roblox.com/messages/sendmessage?Id=" + receiverID, cookies);
            return RobloxUtils.parseToken(response);
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
        public string RetryUrl { get; set; }
    }

    public class RecipientThumbnail
    {
        public bool Final { get; set; }
        public string Url { get; set; }
        public string RetryUrl { get; set; }
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
