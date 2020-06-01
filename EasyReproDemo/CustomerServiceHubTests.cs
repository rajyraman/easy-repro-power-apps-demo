using System;
using Microsoft.Dynamics365.UIAutomation.Api.UCI;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EasyReproDemo
{
    [TestClass]
    public class CustomerServiceHubTests
    {
        private static XrmApp _xrmApp;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            var username = Environment.GetEnvironmentVariable("username")?.ToSecureString() ?? testContext.Properties["username"].ToString().ToSecureString();
            var password = Environment.GetEnvironmentVariable("password")?.ToSecureString() ?? testContext.Properties["password"].ToString().ToSecureString();
            var appUrl = new Uri(Environment.GetEnvironmentVariable("appurl")?.ToString() ?? testContext.Properties["appurl"].ToString());
            var mfa = Environment.GetEnvironmentVariable("mfaToken")?.ToString().ToSecureString() ?? testContext.Properties["mfaToken"].ToString().ToSecureString();

            var client = new WebClient(new BrowserOptions
            {
                EnableRecording = true,
                BrowserType = BrowserType.Chrome,
                PrivateMode = true,
                UCITestMode = false,
                Headless = bool.Parse(testContext.Properties["headless"]?.ToString() ?? "true")
            });
            _xrmApp = new XrmApp(client);
            _xrmApp.OnlineLogin.Login(appUrl, username, password, mfa);

        }

        [ClassCleanup]
        public static void Cleanup()
        {
            _xrmApp.Dispose();
        }

        [TestMethod]
        public void Open_Account_Grid()
        {
            _xrmApp.Navigation.OpenSubArea("Service", "Accounts");
            var accountCount = _xrmApp.Grid.GetGridItems().Count;
            Assert.IsTrue(accountCount > 0);
        }

        [TestMethod]
        public void Check_If_Show_Chart_Button_Exists()
        {
            _xrmApp.Navigation.OpenSubArea("Service", "Contacts");
            var commandBarButtons = _xrmApp.CommandBar.GetCommandValues();
            Assert.IsTrue(commandBarButtons.Value.Contains("Show Chart"));
        }
    }
}
