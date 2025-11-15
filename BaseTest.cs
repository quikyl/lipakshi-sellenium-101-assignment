using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Selenium101.Tests
{
    [Parallelizable(ParallelScope.Children)]
    public abstract class BaseTest
    {
        protected IWebDriver driver;
        protected string sessionId;
        protected string browser, browserVersion, platformName;

        protected void StartRemoteSession(string browserName, string browserVersionInput, string platform)
        {
            browser = browserName;
            browserVersion = browserVersionInput;
            platformName = platform;

            // LambdaTest uses LT:Options nested capability
            var ltOptions = new Dictionary<string, object>()
            {
                { "username", LambdaTestConfig.Username },
                { "accessKey", LambdaTestConfig.AccessKey },
                { "build", "Selenium101-CSharp-Assignment" },
                { "name", $"Selenium101-{Guid.NewGuid()}" },
                { "platformName", platformName },
                { "selenium_version", "4.0.0" },
                { "w3c", true },
                // Enable video, network logs, console logs, visual logs
                { "video", true },
                { "network", true },
                { "console", true },
                { "visual", true },
                { "networkLogs", true }
            };

            var chromeOptions = new ChromeOptions();
            chromeOptions.BrowserVersion = browserVersion;

            // Add LT:Options as required by modern Selenium / LambdaTest
            chromeOptions.AddAdditionalOption("LT:Options", ltOptions);

            driver = new RemoteWebDriver(LambdaTestConfig.HubUrl, chromeOptions.ToCapabilities(), TimeSpan.FromSeconds(600));

            // store session id
            sessionId = (driver as RemoteWebDriver)?.SessionId?.ToString();
            Console.WriteLine($"[LambdaTest] SessionID: {sessionId}  - Browser: {browser} {browserVersion} on {platformName}");
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                if (driver != null)
                {
                    // You can mark test status via JS executor
                    var passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
                    var status = passed ? "passed" : "failed";
                    ((IJavaScriptExecutor)driver).ExecuteScript("lambda-status=" + status);
                }
            }
            catch { /* ignore */ }
            finally
            {
                driver?.Quit();
            }
        }
    }
}
