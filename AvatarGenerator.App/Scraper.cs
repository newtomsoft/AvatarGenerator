namespace AvatarGenerator.App;

public sealed class Scraper
{
    private const string BaseUrl = "https://this-person-does-not-exist.com/en";
    private readonly IWebDriverFactory _webDriverFactory;
    private IWebDriver _webDriver = null!;
    private IJavaScriptExecutor _javaScriptExecutor = null!;
    private const string GeneratedPath = "generated";
    public Scraper(IWebDriverFactory webDriverFactory)
    {
        _webDriverFactory = webDriverFactory;
        Directory.CreateDirectory(GeneratedPath);
    }

    public void BeginScraping()
    {
        _webDriver = _webDriverFactory.CreateDriver();
        _javaScriptExecutor = (IJavaScriptExecutor)_webDriver;
        _ = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(10));
    }

    public void GoToHomePage()
    {
        _webDriver.Manage().Window.Size = new System.Drawing.Size(950, 1000);
        _webDriver.Navigate().GoToUrl(BaseUrl);
        var wait = FluentWait.Create(_webDriver);
        wait.WithTimeout(TimeSpan.FromMilliseconds(30000));
        wait.PollingInterval = TimeSpan.FromMilliseconds(250);
        wait.Until(IsPageLoaded);

        bool IsPageLoaded(IWebDriver webDriver)
        {
            try
            {
                _ = _webDriver.FindElement(By.Id("avatar"));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public void CleanWindow()
    {
        _javaScriptExecutor.ExecuteScript("document.querySelector('.seo-text').style.display = 'none';");
        _javaScriptExecutor.ExecuteScript("document.querySelector('.header-block').style.display = 'none';");
        _javaScriptExecutor.ExecuteScript("document.querySelector('#download').style.display = 'none';");
        _javaScriptExecutor.ExecuteScript("document.querySelector('.footer').style.display = 'none';");
    }

    public async Task GetFace()
    {
        ClickChangeImage();
        await DownloadImage();
    }

    private async Task DownloadImage()
    {
        var elementImage = _webDriver.FindElement(By.Id("avatar"));
        var avatarUrl = elementImage.GetAttribute("src");
        using var httpClient = new HttpClient();
        var streamGot = await httpClient.GetStreamAsync(avatarUrl);
        var avatarFileName = avatarUrl.Split('/')[^1];
        await using var fileStream = new FileStream(Path.Combine("generated", avatarFileName), FileMode.Create, FileAccess.Write);
        await streamGot.CopyToAsync(fileStream);
    }

    public void CloseBrowser(TimeSpan delay)
    {
        Task.Delay(delay).Wait();
        _webDriver.Close();
        _webDriver.Dispose();
    }


    private void ClickChangeImage()
    {
        while (true)
        {
            try
            {
                _javaScriptExecutor.ExecuteScript("reImage();");
                break;
            }
            catch
            {
                // ignored
            }
        }
    }
}