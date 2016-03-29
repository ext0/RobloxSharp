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
    public enum CurrencyType
    {
        ROBUX,
        Tickets
    }
    public class RobloxPurchaseHandler
    {
        public bool requestLimitedPurchase(int assetID, long userAssetOptionId, int expectedPrice, String cookies)
        {
            return requestLimitedPurchase(getToken(assetID, userAssetOptionId, expectedPrice, cookies), assetID, userAssetOptionId, expectedPrice, cookies);
        }
        public bool requestAssetPurchase(long assetID, int expectedPrice, CurrencyType currency, String cookies)
        {
            return requestAssetPurchase(getToken(assetID, expectedPrice, cookies, currency), assetID, expectedPrice, currency, cookies);
        }
        public LimitedPurchaseResponse requestLimitedPurchase(String XRSFToken, int productID, CurrencyType currency, int expectedPrice, int expectedSellerID, int userAssetID, String cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.roblox.com/api/item.ashx?rqtype=purchase&productID=" + productID + "&expectedCurrency=" + ((currency == CurrencyType.ROBUX) ? "1" : "2") + "&expectedPrice=" + expectedPrice + "&expectedSellerID=" + expectedSellerID + "&userAssetID=" + userAssetID);

            request.KeepAlive = true;
            request.Accept = "*/*";
            request.Headers.Add("Origin", @"http://www.roblox.com");
            request.Headers.Add("X-CSRF-TOKEN", XRSFToken);
            request.UserAgent = RobloxUtils.UserAgent;
            request.ContentType = "application/json; charset=utf-8";
            //request.Referer = "http://www.roblox.com/default-item?id=145834328";
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
            request.Headers.Set(HttpRequestHeader.Cookie, cookies);

            request.Method = "POST";
            request.ServicePoint.Expect100Continue = false;

            string body = @"";
            byte[] postBytes = Encoding.UTF8.GetBytes(body);
            request.ContentLength = postBytes.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(postBytes, 0, postBytes.Length);
            stream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream receiveStream = RobloxUtils.decodeStream(response))
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    String s = readStream.ReadToEnd();
                    return JsonConvert.DeserializeObject<LimitedPurchaseResponse>(s);
                }
            }
        }
        internal bool requestLimitedPurchase(String token, int assetID, long userAssetOptionId, int expectedPrice, String cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://m.roblox.com/Catalog/ProcessTransfer");
            request.KeepAlive = true;
            request.Method = "POST";
            request.ServicePoint.Expect100Continue = false;
            request.Accept = "text/html, */*; q=0.01";
            request.Headers.Add("Origin", @"https://m.roblox.com");
            request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
            request.UserAgent = RobloxUtils.UserAgent;
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Referer = "https://m.roblox.com/items/" + assetID + "/privatesales";
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
            request.Headers.Set(HttpRequestHeader.Cookie, cookies);
            string body = @"__RequestVerificationToken=" + token + "&CurrencyType=1&AssetID=0&UserAssetOptionID=" + userAssetOptionId + "&ExpectedPrice=" + expectedPrice;
            byte[] postBytes = Encoding.UTF8.GetBytes(body);
            request.ContentLength = postBytes.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(postBytes, 0, postBytes.Length);
            stream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (BufferedStream receiveStream = new BufferedStream(response.GetResponseStream()))
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    String s = readStream.ReadToEnd();
                    return s.Contains("successfully");
                }
            }
        }
        internal bool requestAssetPurchase(String token, long assetID, int expectedPrice, CurrencyType currency, String cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://m.roblox.com/Catalog/ProcessPurchase");

            request.KeepAlive = true;
            request.Accept = "text/html, */*; q=0.01";
            request.Headers.Add("Origin", @"https://m.roblox.com");
            request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
            request.UserAgent = RobloxUtils.UserAgent;
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            request.Referer = "https://m.roblox.com/items/295133189";
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
            request.Headers.Set(HttpRequestHeader.Cookie, cookies);
            request.Method = "POST";
            request.ServicePoint.Expect100Continue = false;

            string body = @"__RequestVerificationToken=" + token + "&CurrencyType=" + ((currency == CurrencyType.ROBUX) ? "1" : "2") + "&AssetID=" + assetID + "&UserAssetOptionID=0&ExpectedPrice=" + expectedPrice;
            byte[] postBytes = Encoding.UTF8.GetBytes(body);
            request.ContentLength = postBytes.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(postBytes, 0, postBytes.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (BufferedStream receiveStream = new BufferedStream(response.GetResponseStream()))
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    String s = readStream.ReadToEnd();
                    return s.Contains("successfully");
                }
            }
        }
        internal String getToken(int assetID, long userAssetOptionId, int expectedPrice, String cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://m.roblox.com/Catalog/VerifyTransfer?userAssetOptionId=" + userAssetOptionId + "&expectedPrice=" + expectedPrice);

            request.KeepAlive = true;
            request.Accept = "text/html, */*; q=0.01";
            request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
            request.UserAgent = RobloxUtils.UserAgent;
            request.Referer = "https://m.roblox.com/items/" + assetID + "/privatesales";
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
            request.Headers.Set(HttpRequestHeader.Cookie, cookies);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = RobloxUtils.decodeStream(response);
            using (BufferedStream receiveStream = new BufferedStream(responseStream))
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    return RobloxUtils.parseToken(readStream.ReadToEnd());
                }
            }
        }
        internal String getToken(long assetID, int expectedPrice, String cookies, CurrencyType currency)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://m.roblox.com/Catalog/VerifyPurchase?assetid=" + assetID + "&type=" + ((currency == CurrencyType.ROBUX) ? "robux" : "tickets") + "&expectedPrice=" + expectedPrice);

            request.KeepAlive = true;
            request.Accept = "text/html, */*; q=0.01";
            request.Headers.Add("X-Requested-With", @"XMLHttpRequest");
            request.UserAgent = RobloxUtils.UserAgent;
            request.Referer = "https://m.roblox.com/items/" + assetID;
            request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, sdch");
            request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
            request.Headers.Set(HttpRequestHeader.Cookie, cookies);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = RobloxUtils.decodeStream(response);
            using (BufferedStream receiveStream = new BufferedStream(responseStream))
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    String s = readStream.ReadToEnd();
                    if (s.Contains("You already own this item"))
                    {
                        throw new Exception("User already owns asset " + assetID + "!");
                    }
                    return RobloxUtils.parseToken(s);
                }
            }
        }
    }
}
