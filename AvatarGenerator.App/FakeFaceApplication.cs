namespace AvatarGenerator.App;

public class FakeFaceApplication
{
    private readonly IWebDriverFactory _webDriverFactory;

    public FakeFaceApplication(IWebDriverFactory webDriverFactory)
    {
        _webDriverFactory = webDriverFactory;
    }

    public async Task Run(int gamesNumber)
    {
        var scraper = new FakeFaceScraper(_webDriverFactory);
        scraper.BeginScraping();
        await GetFaces(scraper, gamesNumber);
        scraper.CloseBrowser(TimeSpan.FromMilliseconds(800));
    }

    private static async Task GetFaces(FakeFaceScraper fakeFaceScraper, int gamesNumber)
    {
        fakeFaceScraper.GoToHomePage();
        fakeFaceScraper.CleanWindow();

        for (var i = 0; i < gamesNumber; i++)
        {
            await fakeFaceScraper.GetFace();
            await Task.Delay(Random.Shared.Next(1500, 2200));
        }
    }
}