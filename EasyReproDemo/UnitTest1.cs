using System;
using System.Security;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyReproDemo
{
    [TestClass]
    public class UnitTest1
    {
        private static TestContext _testContext;
        private static SecureString _username;
        private static SecureString _password;
        private static Uri _appUrl;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            _testContext = testContext;

            _username = _testContext.Properties["username"].ToString().ToSecureString();
            _password = _testContext.Properties["password"].ToString().ToSecureString();
            _appUrl = new Uri(_testContext.Properties["appurl"].ToString());

        }
        [TestMethod]
        public void TestMethod1()
        {
            var client = new WebClient(new BrowserOptions
            {
                EnableRecording = false,
                BrowserType = BrowserType.Chrome,
                PrivateMode = true,
                PageLoadTimeout = TimeSpan.FromSeconds(2),
            });
            using (var xrmApp = new XrmApp(client))
            {
                xrmApp.OnlineLogin.Login(_appUrl, _username, _password);

                xrmApp.Navigation.OpenApp("Customer Service Hub");

                xrmApp.Navigation.OpenSubArea("Service", "Accounts");

            }
        }
    }
}
