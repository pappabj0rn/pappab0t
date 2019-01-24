using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pappab0t.launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            var buildDate = new DateTime(2000, 1, 1)
                .AddDays(version.Build).AddSeconds(version.Revision * 2);

            Console.WriteLine($"pappab0t.ConsoleRunner {version} ({buildDate:G})");
            try
            {
                var bot = new PappaBot();
                bot.Configure();
                bot.Start();

                var run = true;
                Console.WriteLine("Press X to exit.");
                while (run)
                {
                    var consoleKeyInfo = Console.ReadKey();

                    if (consoleKeyInfo.Key == ConsoleKey.X)
                    {
                        run = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(ex);
                Console.ReadKey();
            }
        }
    }
}
