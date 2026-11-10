using System.Text;
using GuessTheNumber.Application;
using GuessTheNumber.Infrastructure;
using GuessTheNumber.Presentation;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = "Угадай число";

var settingsPath = Path.Combine(FindProjectDirectory(), "settings.json");

var view = new ConsoleGameView();
var controller = new GameController(
    new RandomNumberGenerator(),
    view,
    view,
    new SettingsStore(settingsPath));

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
