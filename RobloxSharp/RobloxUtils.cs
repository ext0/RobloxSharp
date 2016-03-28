using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RobloxSharp
{
    public static class RobloxUtils
    {

        public static String UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.130 Safari/537.36";
        /// <summary>
        /// Fetches an XRSF token from the MyTransactions tab
        /// </summary>
        /// <param name="cookies">Authentication cookie string</param>
        /// <returns>XRSTF Token</returns>
        public static String getXSRFToken(String cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.roblox.com/My/Money.aspx#/#MyTransactions_tab");

            request.KeepAlive = true;
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
            request.UserAgent = RobloxUtils.UserAgent;
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
            request.Headers.Set(HttpRequestHeader.Cookie, cookies);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = decodeStream(response);
            using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
            {
                String s = readStream.ReadToEnd();
                int a = s.IndexOf("setToken") + 10;
                return s.Substring(a, s.IndexOf("')", a) - a);
            }
        }
        /// <summary>
        /// Converts a Unicode String to a .NET string (removes instances of 'u00XX')
        /// </summary>
        /// <param name="input">Unicode string to modify</param>
        /// <returns>.NET encoded string</returns>
        public static string unicodeStringToNET(string input)
        {
            var regex = new Regex(@"[uU]([0-9A-F]{4})", RegexOptions.IgnoreCase);
            return input = regex.Replace(input, match => ((char)int.Parse(match.Groups[1].Value,
              NumberStyles.HexNumber)).ToString());
        }
        /// <summary>
        /// Reads a page into a String
        /// </summary>
        /// <param name="url">URL to read</param>
        /// <param name="cookies">Authentication cookies to use (null if anonymous request)</param>
        /// <returns>The content of the page</returns>
        public static String readPage(String url, String cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = true;
            if (cookies != null)
            {
                request.Headers.Set(HttpRequestHeader.CacheControl, "max-age=0");
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.UserAgent = RobloxUtils.UserAgent;
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
                request.Headers.Set(HttpRequestHeader.Cookie, cookies);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = decodeStream(response);
            using (BufferedStream receiveStream = new BufferedStream(responseStream))
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return readStream.ReadToEnd();
                }
            }
        }
        /// <summary>
        /// Checks for GZIP encoding and decompresses the content.
        /// </summary>
        /// <param name="response">HTTPWebResponse to fetch stream from</param>
        /// <returns>Decoded Stream to read from</returns>
        public static Stream decodeStream(HttpWebResponse response)
        {
            Stream responseStream = response.GetResponseStream();
            if (response.ContentEncoding.ToLower().Contains("gzip"))
            {
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            }
            return responseStream;
        }
        /// <summary>
        /// Gets a general RVT that is used to perform most requests. Automatically added to authCookies, you shouldn't have to call this.
        /// </summary>
        /// <param name="cookies">Authentication cookie string</param>
        /// <returns>Returns a tuple with the response and associated CookieContainer</returns>
        public static Tuple<HttpWebResponse, CookieContainer> getGeneralRequestVerificationToken(CookieContainer container, String cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://m.roblox.com/home");
            request.KeepAlive = true;
            request.Headers.Set(HttpRequestHeader.CacheControl, "max-age=0");
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = RobloxUtils.UserAgent;
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
            request.Headers.Set(HttpRequestHeader.Cookie, cookies);
            request.CookieContainer = container;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return new Tuple<HttpWebResponse, CookieContainer>(response, container);
        }
        /// <summary>
        /// Builds a cookie string valid for header use from a CookieContainer and an HttpWebResponse
        /// </summary>
        /// <param name="cookies">Authentication cookie container</param>
        /// <param name="response">HttpWebResponse to be reading CookieContainer from</param>
        /// <returns>Returns a usable header cookie string</returns>
        public static String buildCookieString(CookieContainer cookies, HttpWebResponse response)
        {
            return buildCookieString(cookies, response.ResponseUri);
        }
        /// <summary>
        /// Builds a cookie string valid for header use from a CookieContainer and a Uri
        /// </summary>
        /// <param name="cookies">Authentication cookie container</param>
        /// <param name="uri">Uri to read cookies from</param>
        /// <returns>Returns a usable header cookie string</returns>
        public static String buildCookieString(CookieContainer cookies, Uri uri)
        {
            String s = String.Empty;
            foreach (Cookie cookie in cookies.GetCookies(uri))
            {
                s += (cookie.Name + "=" + cookie.Value + "; ");
            }
            return s;
        }
        /// <summary>
        /// Builds a cookie string valid for header use from a CookieContainer
        /// </summary>
        /// <param name="cookies">Authentication cookie collection</param>
        /// <returns>Returns a usable header cookie string</returns>
        public static String buildCookieString(CookieCollection cookies)
        {
            String s = String.Empty;
            foreach (Cookie cookie in cookies)
            {
                s += (cookie.Name + "=" + cookie.Value + "; ");
            }
            return s;
        }
        public static UsernameRequest getUsername(String id)
        {
            try
            {
                return JsonConvert.DeserializeObject<UsernameRequest>(readPage("http://api.roblox.com/Users/" + id, ""));
            }
            catch
            {
                return new UsernameRequest
                {
                    AvatarFinal = true,
                    AvatarUri = "",
                    Id = 1,
                    IsOnline = false,
                    Username = "unknown"
                };
            }
        }
        public static UsernameRequest getUserId(String username)
        {
            try
            {
                return JsonConvert.DeserializeObject<UsernameRequest>(readPage("http://api.roblox.com/users/get-by-username?username=" + username, ""));
            }
            catch
            {
                return new UsernameRequest
                {
                    AvatarFinal = true,
                    AvatarUri = "",
                    Id = 1,
                    IsOnline = false,
                    Username = "unknown"
                };
            }
        }
        public static TradeObject makeTradeObject(TradeDetailsData data)
        {
            TradeObject obj = new TradeObject
            {
                receiving = data.AgentOfferList[1],
                sending = data.AgentOfferList[0]
            };
            return obj;
        }
        public static String getUrlForHeadshot(String username)
        {
            try
            {
                UsernameRequest request = getUserId(username);
                HeadShotResponse response = JsonConvert.DeserializeObject<HeadShotResponse>(new WebClient().DownloadString("http://www.roblox.com/headshot-thumbnail/json?userId=" + request.Id + "&width=180&height=180"));
                return response.Url;
            }
            catch
            {
                return "/Images/placeholder.png";
            }
        }
        public static String parseToken(String input)
        {
            if (input.IndexOf("RequestVerificationToken") == -1)
            {
                throw new Exception("Input string (" + input + ") does not contain a valid RVT!");
            }
            int a = input.IndexOf("value", input.IndexOf("RequestVerificationToken")) + 7;
            return input.Substring(a, input.IndexOf("\"", a + 1) - a);
        }
    }

    //Why is this here? 
    //ROBLOX uses this JSON format so often that I may as well put it in the RobloxUtils just for simplicity reasons.
    public class RobloxResponse
    {
        public string sl_translate { get; set; }
        public bool success { get; set; }
        public string msg { get; set; }
        public Object data { get; set; }
    }

    public class HeadShotResponse
    {
        public string Url { get; set; }
        public bool Final { get; set; }
    }

    public class TradeObject
    {
        public UserOfferList receiving { get; set; }
        public UserOfferList sending { get; set; }
    }

    public class RobloxTradeInfoResponse
    {
        public string sl_translate { get; set; }
        public bool success { get; set; }
        public string msg { get; set; }
        public TradeDetailsData data { get; set; }
    }

    public class UsernameRequest
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public object AvatarUri { get; set; }
        public bool AvatarFinal { get; set; }
        public bool IsOnline { get; set; }
    }
}
