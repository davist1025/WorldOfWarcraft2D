using System;
using System.Text;

using var game = new WoW.Client.ClientCore();
game.Run();

try
{
    
}
catch (Exception ex)
{
    var stringBuilder = new StringBuilder();
    stringBuilder.Append("Oh no! :(");
    stringBuilder.Append($"{ex.Message}\n{ex.StackTrace}");

    System.IO.File.WriteAllText("crash.log", stringBuilder.ToString());
}
