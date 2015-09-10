using System;
using RobloxSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Diagnostics;

namespace RobloxSharpTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            CookieContainer collection;
            RobloxLogin login = new RobloxLogin("HomeguardDev", "omit", out collection);
            Debug.WriteLine(login.authCookies);
            RobloxMessageFetcher messageReader = new RobloxMessageFetcher();
            MessageCollection messages = messageReader.getNewMessages("0", login.authCookies);
            foreach (Message message in messages.Collection)
            {
                Debug.WriteLine(message.Sender.UserName + ":" + message.Body);
            }
        }
        public string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
