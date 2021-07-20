using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Firefox;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;
using SeleniumTest1.InfluxDB;
using SeleniumTest1.Pages;

namespace SeleniumTest1
{
    class FirefoxTest : TestBase
    {
        [Test]
        public void TestSearchWiki()
        {
            /*IWebElement searchField = driver.FindElement(By.XPath("//input[@id=\"searchInput\"]"));
            searchField.SendKeys("automation testing");
            IWebElement searchButton = driver.FindElement(By.XPath("//*[@id=\"search-form\"]/fieldset/button"));
            searchButton.Click()*/

            /*var ResponseTime = Convert.ToInt32(js.ExecuteScript("return window.performance.timing.loadEventEnd-window.performance.timing.navigationStart;"));
            Console.WriteLine("Page {0} loading time is {1} ms", driver.Title, ResponseTime);*/


            WikiSearchPage wikiSearchPage = new WikiSearchPage(driver);
            wikiSearchPage.FillSearchField("automation testing");
            MetricsWriter.MeasurePageLoad("Wikipedia", "Search", () => wikiSearchPage.searchButton.Click());
        }
    }
}
