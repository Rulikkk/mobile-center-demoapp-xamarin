namespace MobileCenterDemoApp.UITests
{
    using NUnit.Framework;
    using System;
    using System.Linq;
    using Xamarin.UITest;

    [TestFixture(Platform.Android)]    
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        private IApp _app;
        private readonly Platform _platform;

        public Tests(Platform platform)
        {
            _platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            _app = AppInitializer.StartApp(_platform);            
        }

        [Test]
        public void CheckFacebookButton()
        {
            const string buttonName = "Login via Facebook";

            _app.WaitForElement(x => x.Button());

            var button = _app.Query(x => x.Button(buttonName));
            
            Assert.IsNotEmpty(button);
        }

        [Test]
        public void CheckTwitterButton()
        {
            const string buttonName = "Login via Twitter";

            _app.WaitForElement(x => x.Button());

            var button = _app.Query(x => x.Button(buttonName));

            Assert.IsNotEmpty(button);
        }

        [Test]
        public void CheckLoginPage()
        {
            const string buttonName = "Login via Facebook";

            _app.WaitForElement(x => x.Button());

            _app.Tap(x => x.Button(buttonName));

            _app.WaitForElement(x => x.Css("TITLE"), timeout: TimeSpan.FromSeconds(100));

            var webTitles = _app.Query(x => x.Css("TITLE"));
            Assert.IsNotEmpty(webTitles);

            var webTitle = webTitles.First();            
            Assert.IsTrue(webTitle.TextContent.Contains("Log in to Facebook | Facebook"));


            _app.WaitForElement(x => x.Css("input"), timeout: TimeSpan.FromSeconds(50));

            var inputViewElement = _app.Query(x => x.Css("input"));
            Assert.IsNotEmpty(inputViewElement);
        }
    }
}