var hostBuilder = Host.CreateDefaultBuilder(args);
var host = hostBuilder
    .ConfigureServices((_, services) =>
    {
        services.AddOptions();
        services.AddSingleton<FakeFaceApplication>();
        services.AddSingleton<IWebDriverFactory, FirefoxDriverFactory>();
    })
    .UseConsoleLifetime()
    .Build();

using var serviceScope = host.Services.CreateScope();
var services = serviceScope.ServiceProvider;
var application = services.GetRequiredService<FakeFaceApplication>();

await application.Run(RequestFacesNumber());


static int RequestFacesNumber()
{
    const int minNumber = 1;
    const int maxNumber = 100;
    int number;
    while (true)
    {
        Console.WriteLine($"Number of faces to generate ({minNumber} - {maxNumber}) ?");
        var userInput = Console.ReadLine();
        var isNumber = int.TryParse(userInput, out number);
        if (isNumber && number is >= minNumber and <= maxNumber) break;
    }
    return number;
}