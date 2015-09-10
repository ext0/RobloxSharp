using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web

namespace RobloxSharp
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class RobloxLogin
    {
        /// <summary>
        /// Logs into ROBLOX.com
        /// </summary>
        /// <param name="username">ROBLOX account username</param>
        /// <param name="password">ROBLOX account password</param>
        /// <returns>If successful, returns the login cookies, otherwise returns null.</returns>
        public RobloxLogin(String username, String password)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://m.roblox.com/login");
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(getWebCookies("https://m.roblox.com/"));
            request.KeepAlive = true;
            request.Headers.Set(HttpRequestHeader.CacheControl, "max-age=0");
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Origin", @"https://m.roblox.com");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.132 Safari/537.36";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Referer = "https://m.roblox.com/Login?ReturnUrl=%2f";
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
            request.AllowAutoRedirect = false;

            request.Method = "POST";
            request.ServicePoint.Expect100Continue = false;

            string body = @"UserName=" + username + "&Password=" + password + "&IdentificationCode=";
            byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
            request.ContentLength = postBytes.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(postBytes, 0, postBytes.Length);
            stream.Close();
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
    }
}
