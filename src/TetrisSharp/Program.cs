using System;

namespace TetrisSharp
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new TetrisGame())
                game.Run();
        }
    }
}
