﻿using System;
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
    static class RobloxUtils
    {
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
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.130 Safari/537.36";
            //request.Referer = "http://www.roblox.com/Trade/TradeWindow.aspx?TradeSessionId=19511404&TradePartnerID=49951807";
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
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.130 Safari/537.36";
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
    }
}