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
            RobloxLogin login = new RobloxLogin("HomeguardDev", Reverse("omit"), out collection);
            RobloxPurchaseHandler handler = new RobloxPurchaseHandler();
            Debug.WriteLine(handler.requestLimitedPurchase(145834328, 1704166954, 102, login.authCookies));
        }
        public string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
