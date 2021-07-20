using System;
using System.Security.Policy;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTest1
{
    public class TestBase
    {
        public static IWebDriver driver;

        public static DefaultWait<IWebDriver> fluentWait
        {
            get
            {
                DefaultWait<IWebDriver> _fluentWait = new DefaultWait<IWebDriver>(driver);
                _fluentWait.Timeout = TimeSpan.FromSeconds(10);
                _fluentWait.PollingInterval = TimeSpan.FromMilliseconds(100);
                return _fluentWait;
            }
        }

        public static IJavaScriptExecutor js => (IJavaScriptExecutor) driver;
        string Url = "https://www.wikipedia.org/";

        [SetUp]
        public void startBrowser()
        {
            driver = new FirefoxDriver();

            driver.Manage().Window.Maximize();

            driver.Url = Url;
        }

        [TearDown]
        public void closeBrowser()
        {
            driver.Close();
        }
    }
}
