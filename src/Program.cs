using System;

namespace RaceXNA
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (RacingGame game = new RacingGame())
            {
                game.Run();
            }
        }
    }
}

