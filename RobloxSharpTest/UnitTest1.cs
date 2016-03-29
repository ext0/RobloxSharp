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
            RobloxLogin login = new RobloxLogin("RoJackpotBot", "omit", out collection);
            RobloxPurchaseHandler handler = new RobloxPurchaseHandler();
            Debug.WriteLine(handler.requestLimitedPurchase(
                RobloxUtils.getXSRFToken(login.authCookies),
                19046522,
                CurrencyType.ROBUX,
                105,
                98563581,
                1703607090,
                login.authCookies).TransactionVerb);
        }
        public string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
