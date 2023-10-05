# Spotify Account Checker

Automate the process of verifying the validity of Spotify accounts.

## Description

**Spotify Account Checker** is a tool that checks a list of Spotify accounts to determine their validity. It takes a list of accounts in the format `email:password` and outputs which ones are valid.

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) [Just Use Visual Studio]
- [Selenium WebDriver](https://www.selenium.dev/documentation/en/webdriver/driver_requirements/)
- [Edge WebDriver](https://developer.microsoft.com/en-us/microsoft-edge/tools/webdriver/)

## Usage

1. Create a text file named `account.txt` in the project directory.
2. Input your list of Spotify accounts in the format:
```ruby
email1@example.com:password1
email2@example.com:password2
```
3. Run the program:
4. After processing, valid accounts will be saved in `working.txt`.

## Customisation

- **Thread Count**: By default, the checker processes 5 accounts concurrently. Adjust the thread count by modifying the `threadCount` variable in `Program.cs`:
```csharp
int threadCount = 5; // Change this value as needed
```
- **Retry Logic**: The program retries an account up to 3 times if it encounters a "429 Too Many Requests" error. Adjust these settings in `Program.cs`:
```csharp
int retryCount = 3;  // Adjust number of retries
int delayInSeconds = 10; // Adjust wait time between retries
```
## Changing the Web Driver

By default, this project uses the Edge WebDriver. If you wish to change this to Chrome or Brave, follow the steps below:

### Using Chrome:

1. Install the ChromeDriver NuGet package:
   ```csharp
   dotnet add package Selenium.WebDriver.ChromeDriver
   ```

2. Ensure you have [Chrome Browser](https://www.google.com/chrome/) installed.

3. Replace the Edge WebDriver instantiation in the code:
   ```csharp
   var driverService = EdgeDriverService.CreateDefaultService();
   IWebDriver driver = new EdgeDriver(driverService);
   ```

   with the Chrome WebDriver instantiation:

   ```csharp
   IWebDriver driver = new ChromeDriver();
   ```

### Using Brave:

1. Ensure you have [Brave Browser](https://brave.com/download/) installed.

2. Locate the Brave executable on your system:
   - Common on Windows: `C:\Program Files (x86)\BraveSoftware\Brave-Browser\Application\brave.exe`

3. Use ChromeDriver (Brave is built on the same engine as Chrome), but point it to the Brave browser executable:

   ```csharp
   ChromeOptions options = new ChromeOptions();
   options.BinaryLocation = @"path_to_brave"; // Replace with the path to your Brave executable
   IWebDriver driver = new ChromeDriver(options);
   ```

Replace `path_to_brave` with the appropriate path for your system from step 2.

# Notes
* Ensure you abide by Spotify's terms of service when using this tool.
* Accounts listed in working.txt have been verified as valid at the time of checking but may become invalid in the future due to password changes or other reasons.
