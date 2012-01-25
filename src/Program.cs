using System;

namespace racexna
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

