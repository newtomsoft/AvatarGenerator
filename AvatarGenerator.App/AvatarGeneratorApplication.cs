namespace AvatarGenerator.App;

public class AvatarGeneratorApplication
{
    private readonly IWebDriverFactory _webDriverFactory;

    public AvatarGeneratorApplication(IWebDriverFactory webDriverFactory)
    {
        _webDriverFactory = webDriverFactory;
    }

    public async Task Run(int gamesNumber)
    {
        var scraper = new Scraper(_webDriverFactory);
        scraper.BeginScraping();
        await GetFaces(scraper, gamesNumber);
        scraper.CloseBrowser(TimeSpan.FromMilliseconds(800));
    }

    private static async Task GetFaces(Scraper scraper, int gamesNumber)
    {
        scraper.GoToHomePage();
        scraper.CleanWindow();
        for (var i = 0; i < gamesNumber; i++)
        {
            await scraper.GetFace();
            await Task.Delay(Random.Shared.Next(1500, 2200));
        }
    }
}