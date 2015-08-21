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
//            args = new string[] { "prompt" };

            var ui = new ConsoleUi();
            ui.parseArgs(args);
        }
    }
}
