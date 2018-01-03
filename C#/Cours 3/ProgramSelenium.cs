using System;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver webDriver = new FirefoxDriver();
            webDriver.Navigate().GoToUrl("http://www.ecosia.org");
            for (int numeroDepartementIndex = 1; numeroDepartementIndex < 95; numeroDepartementIndex++)
            {
                IWebElement searchField = webDriver.FindElement(By.Name("q"));
                searchField.Clear();
                searchField.SendKeys(String.Format("CCI {0,2:00}",numeroDepartementIndex));
                IWebElement searchBtn = webDriver.FindElement(By.ClassName("search-form-submit"));
                searchBtn.Click();
                System.Threading.Thread.Sleep(2000);
            }
            
            Console.ReadLine();
            webDriver.Quit();
        }
    }
}
