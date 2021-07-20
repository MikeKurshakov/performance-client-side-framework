using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.PageObjects;

namespace SeleniumTest1.Pages
{
    public class WikiSearchPage
    {
        [FindsBy(How = How.Id, Using = "searchInput")]
        public IWebElement searchField;        
        
        [FindsBy(How = How.XPath, Using = "//*[@id=\"search-form\"]/fieldset/button")]
        public IWebElement searchButton;

        private IWebDriver driver;
        private WebDriverWait wait;

        public WikiSearchPage(IWebDriver driver)
        {
            this.driver = driver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            PageFactory.InitElements(driver, this);
        }
        public SearchPage FillSearchField(string inputSearch)
        {
            searchField.SendKeys(inputSearch);
            return new SearchPage(driver);
        }
    }
}
