using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RobloxSharp
{
    public enum TradeType
    {
        Inactive,
        Inbound,
        Outbound,
        Completed
    }
    public enum TradeResponseType
    {
        Accept,
        Send,
        Decline
    }
    public enum HatType
    {
        HAT,
        GEAR,
        FACE
    }
    public class RobloxTradeHandler
    {
        /// <summary>
        /// Creates a JSON trade request with the supplied parameters.
        /// </summary>
        /// <returns>Returns a usable JSON string</returns>
        public String createTradeRequest(String senderID, String receiverID, List<InventoryItem> sendingItems, List<InventoryItem> receivingItems, int sendingRobux, int receivingRobux)
        {
            TradeOffer offer = new TradeOffer();
            offer.IsActive = false;
            offer.TradeStatus = "Open";
            offer.UserOfferList = new List<UserOfferList>();
            offer.UserOfferList.Add(new UserOfferList
            {
                AgentID = int.Parse(senderID),
                OfferList = sendingItems,
                OfferRobux = sendingRobux,
                OfferValue = 0 //placeholder
            });
            offer.UserOfferList.Add(new UserOfferList
            {
                AgentID = int.Parse(receiverID),
                OfferList = receivingItems,
                OfferRobux = receivingRobux,
                OfferValue = 0 //placeholder
            });
            return WebUtility.UrlEncode(JsonConvert.SerializeObject(offer));
        }
        public List<InventoryItemResponse> getInventory(String userId, String cookies)
        {
            RobloxTradeHandler handler = new RobloxTradeHandler();
            List<InventoryItemResponse> items = new List<InventoryItemResponse>();
            int i = 1;
            foreach (HatType type in Enum.GetValues(typeof(HatType)))
            {
                i = 1;
                InventoryResponse response = handler.getInventory(userId, i.ToString(), type, cookies);
                while (response != null)
                {
                    if (response.data == null || response.data.InventoryItems == null)
                    {
                        break;
                    }
                    foreach (InventoryItemResponse item in response.data.InventoryItems)
                    {
                        items.Add(item);
                    }
                    i++;
                    response = handler.getInventory(userId, i.ToString(), type, cookies);
                }
            }
            return items;
        }
        public String createTradeRequest(String senderID, String receiverID, TradeObject trade)
        {
            return createTradeRequest(senderID, receiverID, trade.sending.OfferList, trade.receiving.OfferList, trade.sending.OfferRobux, trade.receiving.OfferRobux);
        }
        public RobloxTradeInfoResponse getTradeInfo(String tradeSessionId, String XRSFToken, String cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.roblox.com/Trade/TradeHandler.ashx");

            request.UserAgent = RobloxUtils.UserAgent;
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Headers.Add("X-CSRF-TOKEN", XRSFToken);
            request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
            request.Referer = "http://www.roblox.com/My/Money.aspx";
            request.Headers.Set(HttpRequestHeader.Cookie, cookies);
            request.KeepAlive = true;
            request.Headers.Set(HttpRequestHeader.Pragma, "no-cache");
            request.Headers.Set(HttpRequestHeader.CacheControl, "no-cache");

            request.Method = "POST";
            request.ServicePoint.Expect100Continue = false;

            string body = @"TradeID=" + tradeSessionId + "&cmd=pull";
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
                    String s = readStream.ReadToEnd().Replace("\\\\", "").Replace("\\\"", "\"").Replace("\"{", "{").Replace("}\"", "}");
                    return JsonConvert.DeserializeObject<RobloxTradeInfoResponse>(RobloxUtils.unicodeStringToNET(s));
                }
            }
        }
        public InventoryResponse getInventory(String id, String page, HatType filter, String cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.roblox.com/Trade/InventoryHandler.ashx?token=%22&filter=" + (int)filter + "&userid=" + id + "&page=" + page + "&itemsPerPage=14&_=0");

            request.KeepAlive = true;
            request.Headers.Set(HttpRequestHeader.CacheControl, "max-age=0");
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Upgrade-Insecure-Requests", @"1");
            request.UserAgent = RobloxUtils.UserAgent;
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
            request.Headers.Set(HttpRequestHeader.Cookie, cookies);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream receiveStream = RobloxUtils.decodeStream(response))
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<InventoryResponse>(readStream.ReadToEnd());
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }
        public TradeList fetchTrades(String cookies, String XRSFToken, TradeType type, int startIndex)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.roblox.com/My/Money.aspx/GetMyItemTrades");

            request.UserAgent = RobloxUtils.UserAgent;
            request.Accept = "*/*";
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("X-CSRF-TOKEN", XRSFToken);
            request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
            request.Referer = "http://www.roblox.com/My/Money.aspx";
            request.Headers.Set(HttpRequestHeader.Cookie, cookies);
            request.KeepAlive = true;
            request.Headers.Set(HttpRequestHeader.Pragma, "no-cache");
            request.Headers.Set(HttpRequestHeader.CacheControl, "no-cache");

            request.Method = "POST";
            request.ServicePoint.Expect100Continue = false;

            String a = "";
            if (type == TradeType.Completed)
            {
                a = "completed";
            }
            else if (type == TradeType.Outbound)
            {
                a = "outbound";
            }
            else if (type == TradeType.Inbound)
            {
                a = "inbound";
            }
            else if (type == TradeType.Inactive)
            {
                a = "inactive";
            }
            string body = "{\"statustype\":\"" + a + "\",\"startindex\":" + startIndex + "}";
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
            request.ContentLength = postBytes.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(postBytes, 0, postBytes.Length);
            stream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = RobloxUtils.decodeStream(response);
            using (BufferedStream receiveStream = new BufferedStream(responseStream))
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    String s = readStream.ReadToEnd().Replace("\\\\", "").Replace("\\\"", "\"").Replace("\"{", "{").Replace("}\"", "}");
                    RootTradeList obj = JsonConvert.DeserializeObject<RootTradeList>(s);
                    return obj.d;
                }
            }
        }
        public RobloxResponse sendTrade(TradeResponseType type, String tradeJSON, String XRSFToken, String tradeID, String cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.roblox.com/Trade/TradeHandler.ashx");
            request.KeepAlive = true;
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Add("Origin", @"http://www.roblox.com");
            request.Headers.Add("X-CSRF-TOKEN", XRSFToken);
            request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
            request.UserAgent = RobloxUtils.UserAgent;
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
            request.Headers.Set(HttpRequestHeader.Pragma, "no-cache");
            request.Headers.Set(HttpRequestHeader.CacheControl, "no-cache");
            request.Headers.Set(HttpRequestHeader.Cookie, cookies);
            request.Method = "POST";
            request.ServicePoint.Expect100Continue = false;
            String body = "";
            if (type == TradeResponseType.Accept)
            {
                body = "TradeID=" + tradeID + "&cmd=maketrade&TradeJSON=" + tradeJSON;
            }
            else if (type == TradeResponseType.Decline)
            {
                body = "TradeID=" + tradeID + "&cmd=decline";
            }
            else if (type == TradeResponseType.Send)
            {
                body = "cmd=send&TradeJSON=" + tradeJSON;
            }
            byte[] postBytes = Encoding.UTF8.GetBytes(body);
            request.ContentLength = postBytes.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(postBytes, 0, postBytes.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = RobloxUtils.decodeStream(response);
            using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
            {
                RobloxResponse robloxResponse = JsonConvert.DeserializeObject<RobloxResponse>(readStream.ReadToEnd());
                if (robloxResponse.data is TradeResponseInfo)
                {
                    robloxResponse.data = (TradeResponseInfo)robloxResponse.data;
                }
                return robloxResponse;
            }
        }
    }


    //Anyone looking past these comments, most of these classes are here simply to ease the process from moving from ROBLOX json response to C# objects.
    //While this may seem overkill, it helps with the abstraction.
    //You should only need to use TradeSession, TradeList, InventoryItem, and UserOfferList

    //For any confusion as to why this class is even here, see here
    //http://stackoverflow.com/questions/830112/what-does-d-in-json-mean

    public class InventoryItemResponse
    {
        public string Name { get; set; }
        public string ImageLink { get; set; }
        public string ItemLink { get; set; }
        public string SerialNumber { get; set; }
        public string SerialNumberTotal { get; set; }
        public string AveragePrice { get; set; }
        public string OriginalPrice { get; set; }
        public string UserAssetID { get; set; }
        public object MembershipLevel { get; set; }
        public HatType HatType { get; set; }
    }

    public class InventoryData
    {
        public int agentID { get; set; }
        public int totalNumber { get; set; }
        public List<InventoryItemResponse> InventoryItems { get; set; }
    }

    public class InventoryResponse
    {
        public string sl_translate { get; set; }
        public bool success { get; set; }
        public string msg { get; set; }
        public InventoryData data { get; set; }
    }

    public class RootTradeList
    {
        public TradeList d { get; set; }
    }
    public class TradeList
    {
        public List<TradeSession> Data { get; set; }
        public string totalCount { get; set; }
        public string tradeWriteEnabled { get; set; }
    }
    public class TradeSession
    {
        public string Date { get; set; }
        public string Expires { get; set; }
        public string TradePartner { get; set; }
        public string TradePartnerID { get; set; }
        public string Status { get; set; }
        public string StatusAddon { get; set; }
        public string TradeSessionID { get; set; }
        public string sl_translate { get; set; }
    }
    public class InventoryItem
    {
        public string Name { get; set; }
        public string ImageLink { get; set; }
        public string ItemLink { get; set; }
        public string SerialNumber { get; set; }
        public string SerialNumberTotal { get; set; }
        public string AveragePrice { get; set; }
        public string OriginalPrice { get; set; }
        public string UserAssetID { get; set; }
        public string MembershipLevel { get; set; }
    }
    public class TradeOffer
    {
        public List<UserOfferList> UserOfferList { get; set; }
        public bool IsActive { get; set; }
        public string TradeStatus { get; set; }
    }
    public class Data
    {
        public int agentID { get; set; }
        public int totalNumber { get; set; }
        public List<InventoryItem> InventoryItems { get; set; }
    }
    public class UserOfferList
    {
        public int AgentID { get; set; }
        public List<InventoryItem> OfferList { get; set; }
        public int OfferRobux { get; set; }
        public int OfferValue { get; set; }
    }
    public class TradeDetailsData
    {
        public List<UserOfferList> AgentOfferList { get; set; }
        public bool IsActive { get; set; }
        public string StatusType { get; set; }
        public DateTime Expiration { get; set; }
    }
    public class TradeResponseInfo
    {
        public int agentID { get; set; }
        public int totalNumber { get; set; }
        public List<InventoryItem> InventoryItems { get; set; }
    }
}
