using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRTS
{
    class _Program
    {        
        static int Main(string[] args)
        {
            if (args.Count() != 1)
            {
                Console.WriteLine("Usage: SRTS AssemblyName");
                return 1;
            }

            var path = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), args[0]);
            var asm = System.Reflection.Assembly.LoadFile(path);

            Console.WriteLine(SignalRModule.Create(asm));

            return 0;
        }
    }
}
