using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Web;

namespace RobloxSharp
{
    public class RobloxLogin
    {
        public String authCookies { get; set; }
        public bool successful { get; set; }
        /// <summary>
        /// Attempts to login, sets authCookies to the cookie string needed to perform authenticated tasks if successful.
        /// </summary>
        /// <param name="username">ROBLOX account username</param>
        /// <param name="password">ROBLOX account password</param>
        /// <param name="cookies">CookieContainer storing SetCookie values</param>
        public RobloxLogin(String username, String password, out CookieContainer cookies)
        {
            cookies = new CookieContainer();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://m.roblox.com/login");
            request.CookieContainer = cookies;
            request.KeepAlive = true;
            request.Headers.Set(HttpRequestHeader.CacheControl, "max-age=0");
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Origin", @"https://m.roblox.com");
            request.UserAgent = RobloxUtils.UserAgent;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Referer = "https://m.roblox.com/Login?ReturnUrl=%2f";
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
            request.AllowAutoRedirect = false;

            request.Method = "POST";
            request.ServicePoint.Expect100Continue = false;

            string body = @"UserName=" + HttpUtility.UrlEncode(username) + "&Password=" + HttpUtility.UrlEncode(password) + "&IdentificationCode=";
            byte[] postBytes = Encoding.UTF8.GetBytes(body);
            request.ContentLength = postBytes.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(postBytes, 0, postBytes.Length);
            stream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream responseStream = RobloxUtils.decodeStream(response))
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {

                }
            }
            Tuple<HttpWebResponse, CookieContainer> tuple = RobloxUtils.getGeneralRequestVerificationToken(cookies, RobloxUtils.buildCookieString(cookies, response));
            foreach (Cookie cookie in tuple.Item2.GetCookies(response.ResponseUri))
            {
                if (cookie.Name.Equals(".ROBLOSECURITY"))
                {
                    successful = true;
                    break;
                }
            }
            authCookies = RobloxUtils.buildCookieString(tuple.Item2, tuple.Item1);
        }
    }
}
