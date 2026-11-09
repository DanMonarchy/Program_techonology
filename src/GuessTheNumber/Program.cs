using System.Text;
using GuessTheNumber.Application;
using GuessTheNumber.Infrastructure;
using GuessTheNumber.Presentation;

Console.OutputEncoding = Encoding.UTF8;
Console.Title = "Угадай число";

var view = new ConsoleGameView();
var controller = new GameController(
    new RandomNumberGenerator(),
    view,
    view);

controller.Run();
