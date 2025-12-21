using System;

try
{
    using var game = new WoW.Client.Game1();
    game.Run();
}
catch (Exception ex)
{
    System.IO.File.WriteAllText("crash.log", ex.Message);
}
