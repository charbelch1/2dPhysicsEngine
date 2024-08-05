using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatPhysicsTester
{
    internal class Program
    {
        static void Main()
        {
            using (var game = new Game1())
                game.Run();
        }
    }
}
