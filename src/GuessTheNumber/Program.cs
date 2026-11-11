using System.Text;
using GuessTheNumber.Application;
using GuessTheNumber.Infrastructure;
using GuessTheNumber.Presentation;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = "Угадай число";

var projectDirectory = FindProjectDirectory();
var settingsPath = Path.Combine(projectDirectory, "settings.json");
var statisticsPath = Path.Combine(projectDirectory, "statistics.json");

var view = new ConsoleGameView();
var controller = new GameController(
    new RandomNumberGenerator(),
    view,
    view,
    new SettingsStore(settingsPath),
    new StatisticsStore(statisticsPath));

controller.Run();

// Настройки лежат рядом с файлом проекта, а не в AppData.
static string FindProjectDirectory()
{
    var directory = new DirectoryInfo(AppContext.BaseDirectory);

    while (directory is not null)
    {
        if (File.Exists(Path.Combine(directory.FullName, "GuessTheNumber.csproj")))
        {
            return directory.FullName;
        }

        directory = directory.Parent;
    }

    return Directory.GetCurrentDirectory();
}
