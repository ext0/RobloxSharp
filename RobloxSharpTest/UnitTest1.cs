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
            RobloxLogin login = new RobloxLogin("HomeguardDev", "0", out collection);
            RobloxTradeHandler tradeHandler = new RobloxTradeHandler();
            String XRSFToken = RobloxUtils.getXSRFToken(login.authCookies);
            TradeList list = tradeHandler.fetchTrades(login.authCookies, XRSFToken, TradeType.Inactive, 0);
            foreach (TradeSession tradeSession in list.Data)
            {
                TradeDetailsData info = tradeHandler.getTradeInfo(tradeSession.TradeSessionID, XRSFToken, login.authCookies);
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
