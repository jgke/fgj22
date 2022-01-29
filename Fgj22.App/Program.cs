using System;
using Serilog;

namespace Fgj22.App
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                .Destructure.With(new LoggableDestructuringPolicy())
                .WriteTo.Console()
                .CreateLogger();

            using (var game = new Game1())
                game.Run();
        }
    }
}
