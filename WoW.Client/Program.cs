using System;
using System.Text;

try
{
    using var game = new WoW.Client.Game1();
    game.Run();
}
catch (Exception ex)
{
    var stringBuilder = new StringBuilder();
    stringBuilder.Append("Oh no! :(");
    stringBuilder.Append($"{ex.Message}\n{ex.StackTrace}");

    System.IO.File.WriteAllText("crash.log", stringBuilder.ToString());
}
