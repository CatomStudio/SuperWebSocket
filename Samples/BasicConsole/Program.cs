using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;

namespace SuperWebSocket.Samples.BasicConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start the WebSocketServer!");

            Console.ReadKey();
            Console.WriteLine();

            var appServer = new WebSocketServer();

            //Setup the appServer
            if (!appServer.Setup(2012)) //Setup with listening port
            {
                Console.WriteLine("Failed to setup!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine();

            //Try to start the appServer
            if (!appServer.Start())
            {
                Console.WriteLine("Failed to start!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("The server started successfully, press key 'q' to stop it!");

            // WS服务器时间 & 操作
            appServer.NewSessionConnected += appServer_NewSessionConnected;
            appServer.NewMessageReceived += appServer_NewMessageReceived;
            appServer.SessionClosed += appServer_SessionClosed;

            // 循环监听
            while (Console.ReadKey().KeyChar != 'q')
            {
                var key = Console.ReadLine();
                appServer.NewMessageReceived += (session, message) =>
                {
                    session.Send(string.Format("[{0}] Server: I have got your msg: [ {1} ].", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), key));
                };
                continue;
            }

            //Stop the appServer
            appServer.Stop();

            Console.WriteLine();
            Console.WriteLine("The server was stopped!");
            Console.ReadKey();
        }

        // When receive message.
        static void appServer_NewMessageReceived(WebSocketSession session, string message)
        {
            //Receive the message
            Console.WriteLine("[{0}] Client: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message);
            //Send the received message back
            session.Send(string.Format("[{0}] Server: I have got your msg: [ {1} ].", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message));
        }
        // When client connected
        static void appServer_NewSessionConnected(WebSocketSession session)
        {
            session.Send(string.Format("[{0}] Server: Connected! ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        }
        // When server closed
        static void appServer_SessionClosed(WebSocketSession session, CloseReason reason)
        {
            if (reason == CloseReason.ServerShutdown) return;
            session.Send(string.Format("[{0}] Server: will closed! close reason: {1} .", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), session.Cookies["name"]));
        }

    }
}
