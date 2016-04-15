using System;
using RobloxSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

namespace RobloxSharpTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            CookieContainer collection;
            RobloxLogin login = new RobloxLogin("HomeguardDev","omit", out collection);
            Debug.WriteLine(login.successful);
        }
    }
}
