using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace SeleniumTest1.Pages
{
    public class SearchPage
    {
        private IWebDriver driver;

        public SearchPage(IWebDriver driver)
        {
            this.driver = driver;
            PageFactory.InitElements(driver, this);
        }
    }
}
