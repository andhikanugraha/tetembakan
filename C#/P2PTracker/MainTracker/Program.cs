using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            bool shutdown = false;
            while(!shutdown)
            {
                Console.Write("> ");
                string s = Console.ReadLine();
                if (s.Equals("shutdown"))
                {
                    shutdown = true;
                }
                Console.WriteLine(Encoding.Default.GetByteCount(s.Split(' ')[0]));
            }
        }
    }
}
