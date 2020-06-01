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
        private static WebClient _webClient;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            var username = Environment.GetEnvironmentVariable("powerappsusername")?.ToSecureString() ?? testContext.Properties["powerappsusername"].ToString().ToSecureString();
            var password = Environment.GetEnvironmentVariable("powerappspassword")?.ToSecureString() ?? testContext.Properties["powerappspassword"].ToString().ToSecureString();
            var appUrl = new Uri(Environment.GetEnvironmentVariable("appurl")?.ToString() ?? testContext.Properties["appurl"].ToString());
            var mfa = Environment.GetEnvironmentVariable("mfaToken")?.ToString().ToSecureString() ?? testContext.Properties["mfaToken"].ToString().ToSecureString();

            _webClient = new WebClient(new BrowserOptions
            {
                EnableRecording = true,
                BrowserType = BrowserType.Chrome,
                PrivateMode = true,
                UCITestMode = false,
                Headless = bool.Parse(testContext.Properties["headless"]?.ToString() ?? "true")
            });
            _xrmApp = new XrmApp(_webClient);
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
        public void QuickCreate_Contact()
        {
            _xrmApp.Navigation.QuickCreate("contact");
            _xrmApp.QuickCreate.SetValue("firstname", "Easy");
            _xrmApp.QuickCreate.SetValue("lastname", "Repro");
            _xrmApp.QuickCreate.Save();
            _xrmApp.Navigation.OpenSubArea("Service", "Contacts");
            _xrmApp.Grid.Search("Easy");
            var contacts = _xrmApp.Grid.GetGridItems();
            Assert.IsTrue(contacts.Count > 0);

            _xrmApp.Grid.OpenRecord(0);
            _webClient.Browser.Driver.ExecuteScript("Xrm.WebApi.deleteRecord('contact', Xrm.Utility.getPageContext().input.entityId).then(x=>Xrm.Navigation.navigateTo({pageType: 'entitylist', entityName: 'contact'}))");
            _xrmApp.Grid.Search("Easy");
            Assert.IsTrue(_xrmApp.Grid.GetGridItems().Count == 0);
        }
    }
}
