using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace Selenium101.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    public class Selenium101Tests : BaseTest
    {
        // TestCase arguments:
        // browser, browserVersion, platformName
        [TestCase("chrome", "120.0", "Windows 10")]
        [TestCase("chrome", "120.0", "MacOS Monterey")] // change one to Safari if you want; using Chrome on Mac for reliability
        public void RunAllScenarios(string browserName, string browserVersion, string platformName)
        {
            StartRemoteSession(browserName, browserVersion, platformName);

            Console.WriteLine("SessionID exposed at start: " + sessionId);

            // Scenario 1
            Scenario1_SimpleFormDemo();

            // Scenario 2
            Scenario2_DragAndDropSlider();

            // Scenario 3
            Scenario3_FormSubmitValidation();

            // Print final session id again to ensure you can capture it from console/video logs
            Console.WriteLine($"Final Session ID for submission: {sessionId}");
        }

        private void Scenario1_SimpleFormDemo()
        {
            // 1. Open playground
            driver.Navigate().GoToUrl("https://www.lambdatest.com/selenium-playground");
            driver.Manage().Window.Maximize();
            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(d => d.Title.ToLower().Contains("selenium playground"));

            // 2. Click "Simple Form Demo" (locator: link text)
            var simpleFormLink = driver.FindElement(By.LinkText("Simple Form Demo"));
            simpleFormLink.Click();

            // 3. Validate URL contains simple-form-demo
            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(d => d.Url.Contains("simple-form-demo"));
            Assert.IsTrue(driver.Url.Contains("simple-form-demo"), "URL does not contain simple-form-demo");

            // 4-6. Enter message and click "Get Checked Value"
            string message = "Welcome to LambdaTest";
            var inputBox = driver.FindElement(By.CssSelector("#user-message")); // css selector
            inputBox.Clear();
            inputBox.SendKeys(message);

            var showButton = driver.FindElement(By.XPath("//button[text()='Get Checked Value']")); // xpath
            showButton.Click();

            // 7. Validate message on right panel
            var displayed = new WebDriverWait(driver, TimeSpan.FromSeconds(5)).Until(d => d.FindElement(By.Id("message")).Displayed);
            var displayedText = driver.FindElement(By.Id("message")).Text;
            Assert.AreEqual(message, displayedText, "Displayed message mismatch in Simple Form Demo");

            Console.WriteLine("Scenario 1 passed");
        }

        private void Scenario2_DragAndDropSlider()
        {
            driver.Navigate().GoToUrl("https://www.lambdatest.com/selenium-playground");
            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(d => d.Title.ToLower().Contains("selenium playground"));

            // Click "Drag & Drop Sliders"
            var slidersLink = driver.FindElement(By.LinkText("Drag & Drop Sliders"));
            slidersLink.Click();

            // Wait for sliders page to load
            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(d => d.Url.Contains("drag-drop-range-sliders"));

            // Locate the slider with label "Default value 15"
            // There are multiple sliders — we'll use XPath to pick the specific input for default value 15
            var sliderInput = driver.FindElement(By.XPath("//div[label[contains(text(),'Default value 15')]]//input[@type='range']"));

            // Move slider to 95. We'll do a loop to send ArrowRight keys until value becomes 95
            var valueLocator = driver.FindElement(By.Id("rangeSuccess")); // the range value display id on page
            int tries = 0;
            Actions actions = new Actions(driver);

            // click the slider to focus
            actions.MoveToElement(sliderInput).Click().Perform();
            Thread.Sleep(300);

            // The slider's value attribute might be 15 initially; we'll move right by sending ArrowRight until 95
            while (tries < 200)
            {
                string valStr = sliderInput.GetAttribute("value");
                if (Int32.TryParse(valStr, out int cur) && cur >= 95) break;

                sliderInput.SendKeys(Keys.ArrowRight);
                tries++;
            }

            // Validate value shows 95
            string finalValue = sliderInput.GetAttribute("value");
            Console.WriteLine("Slider value after moves: " + finalValue);
            Assert.IsTrue(int.TryParse(finalValue, out int fv) && fv >= 95, $"Slider final value is {finalValue}, expected >= 95");

            Console.WriteLine("Scenario 2 passed");
        }

        private void Scenario3_FormSubmitValidation()
        {
            driver.Navigate().GoToUrl("https://www.lambdatest.com/selenium-playground");
            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(d => d.Title.ToLower().Contains("selenium playground"));

            // Click "Input Form Submit"
            var inputFormLink = driver.FindElement(By.LinkText("Input Form Submit"));
            inputFormLink.Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(d => d.Url.Contains("input-form-demo"));

            // 2. Click Submit without filling
            var submitButton = driver.FindElement(By.CssSelector("button#submit"));
            submitButton.Click();

            // 3. Assert "Please fill out this field." HTML5 validation pops up — we can check validity
            var nameField = driver.FindElement(By.Name("name"));
            // In many browsers, calling GetAttribute("validationMessage") yields the message when invalid
            string validationMsg = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].validationMessage;", nameField);
            Console.WriteLine("Validation message: " + validationMsg);
            Assert.IsTrue(!string.IsNullOrEmpty(validationMsg), "Expected HTML5 validation message for required fields");

            // 4. Fill fields including some different locators (id, name, css)
            nameField.SendKeys("Test User");
            driver.FindElement(By.Name("email")).SendKeys($"test{Guid.NewGuid().ToString().Substring(0,5)}@example.com");
            driver.FindElement(By.Name("password")).SendKeys("Password123!");
            driver.FindElement(By.Name("company")).SendKeys("LambdaTest");
            driver.FindElement(By.Name("website")).SendKeys("https://example.com");
            driver.FindElement(By.Name("city")).SendKeys("New York");
            driver.FindElement(By.Name("address1")).SendKeys("Line 1");
            driver.FindElement(By.Name("address2")).SendKeys("Line 2");
            driver.FindElement(By.Name("state")).SendKeys("NY");
            driver.FindElement(By.Name("zip")).SendKeys("10001");

            // 5. Select Country = United States using text — using SelectElement
            var countrySelectElem = driver.FindElement(By.Name("country"));
            var select = new OpenQA.Selenium.Support.UI.SelectElement(countrySelectElem);
            select.SelectByText("United States");

            // 6. Click Submit
            submitButton.Click();

            // 7. Validate success message
            new WebDriverWait(driver, TimeSpan.FromSeconds(10)).Until(d =>
            {
                try
                {
                    var e = d.FindElement(By.CssSelector(".success-msg"));
                    return e.Displayed && e.Text.Contains("Thanks for contacting us");
                }
                catch { return false; }
            });

            var success = driver.FindElement(By.CssSelector(".success-msg")).Text;
            Assert.IsTrue(success.Contains("Thanks for contacting us"), "Success message not found");

            Console.WriteLine("Scenario 3 passed");
        }
    }
}
