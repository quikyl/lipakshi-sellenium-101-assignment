using System;

namespace Selenium101.Tests
{
    public static class LambdaTestConfig
    {
        public static string Username => Environment.GetEnvironmentVariable("LT_USERNAME") ?? throw new Exception("Set LT_USERNAME");
        public static string AccessKey => Environment.GetEnvironmentVariable("LT_ACCESS_KEY") ?? throw new Exception("Set LT_ACCESS_KEY");

        // LambdaTest hub URL
        public static Uri HubUrl => new Uri($"https://{Username}:{AccessKey}@hub.lambdatest.com/wd/hub");
    }
}
