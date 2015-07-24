using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guru
{
    class Program
    {
        static void Main(string[] args)
        {


            //			args = new string[] {"?", "Smile" };

            args = new string[] { ">" };

            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }

            var ui = new ConsoleUi();
            ui.parseArgs(args);
        }
    }
}
