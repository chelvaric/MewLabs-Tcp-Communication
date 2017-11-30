using System;
using MewLabTcpCommunication.Server.Models;
using MewLabsWebScoketProtocolParser;
using System.Net;
using System.Threading;

namespace ExampleWebSocketProject
{
    class Program
    {
        static void Main(string[] args)
        {
            //make the server
            MewLabsServer server = new MewLabsServer(IPAddress.Any,MEWLABS_COMMUNICATION_TYPE.TCP,8080,new MewLabsWebSocketParser());

            server.OnMessageDecoded += Server_OnMessageDecoded;

          Thread t = new Thread(()=> { server.Start().Wait(); } );

            t.Start();

           string line = Console.ReadLine();

            if(line == "quit" || line =="q" || line == "exit")
            {
                server.Stop();
                t.Join();

            }

        }

        private static void Server_OnMessageDecoded(MewLabTcpCommunication.Server.Events.MessageParsedEventArgs eventargs)
        {
            Console.WriteLine(eventargs.Client.ClientID + " recieved message " + eventargs.Command.ToString());
        }
    }
}
