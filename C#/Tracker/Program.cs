using System;
using System.Text;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Collections.Generic;

namespace MainTracker
{
    class Program
    {
        public static void Main(string[] args)
        {
            Server server = new Server();
            bool shutdown = false;
            while (!shutdown)
            {
                Console.Write("> ");
                string s = Console.ReadLine();
                string[] command = s.Split(' ');
                Console.WriteLine(command[0]);
                if (command[0].Equals("shutdown"))
                {
                    shutdown = true;
                }
                if (command[0].Equals("log"))
                {
                    if (command.Length < 2)
                    {
                        Console.WriteLine("Unknown parameter");
                    } else
                    if (command[1].Equals("on"))
                    {
                        server.log = true;
                        Console.WriteLine("Log turned on");
                    }
                    else if (command[1].Equals("off"))
                    {
                        server.log = false;
                        Console.WriteLine("Log turned off");
                    }
                    else
                    {
                        Console.WriteLine("Unknown parameter");
                    }
                }
                if (command[0].Equals("max_peer"))
                {
                    server.max_peer = Int32.Parse(command[1]);
                    Console.WriteLine("Max peer changed into "+server.max_peer);
                }
                if (command[0].Equals("max_room"))
                {
                    server.max_room = Int32.Parse(command[1]);
                    Console.WriteLine("Max room changed into " + server.max_room);
                }
                Console.WriteLine(Encoding.Default.GetByteCount(s.Split(' ')[0]));
            }
            Console.WriteLine("Program exited");
            Environment.Exit(0);
        }

    }

}
