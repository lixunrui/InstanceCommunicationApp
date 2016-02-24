using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        static Server _server;
        static AutoResetEvent _serverEvent = new AutoResetEvent(false);
        static Thread serverThread;

        static void Main(string[] args)
        {
            InitializeServer();
            StartServer();
            RunOtherTask();

        }

        private static void RunOtherTask()
        {
            
                Console.WriteLine("Enter ESC to exit app");

                ConsoleKeyInfo keyInfo = Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    _serverEvent.Set();
                }
           
            //_serverEvent.WaitOne();
            EndServer();
        }

        private static void EndServer()
        {
            if (serverThread != null)
            {
                if (serverThread.IsAlive)
                {
                    Console.WriteLine("Stop the Server");
                    serverThread.Join(3000);
                  //  serverThread.Abort();

                    Console.WriteLine("Server Stopped");
                }
            }
        }

        private static void StartServer()
        {
            if (_server != null)
            {
                // run the server in a separate thread
                serverThread = new Thread(()=>_server.Start(_serverEvent));
                serverThread.Start();
            }
            else
            {
                Console.WriteLine("Server not available, failed to start");
            }
        }

        private static void InitializeServer()
        {
            Console.WriteLine("Initializing Server...");
            _server = new Server();
            Console.WriteLine("Initialization Completed.");
        }
    }
}
