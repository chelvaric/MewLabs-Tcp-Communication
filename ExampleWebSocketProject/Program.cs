using System;
using NekoIOLabsTcpCommunication.Server.Models;
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
            NekoIOLabsServer server = new NekoIOLabsServer(IPAddress.Any,NEKOIOLABS_COMMUNICATION_TYPE.TCP,8080,new NekoIOLabsWebSocketParser());

            server.OnMessageDecoded += Server_OnMessageDecoded;

              server.Start();

          ;

           string line = Console.ReadLine();

            if(line == "quit" || line =="q" || line == "exit")
            {
                server.Stop();
              

            }

        }

        private static void Server_OnMessageDecoded(NekoIOLabsTcpCommunication.Server.Events.MessageParsedEventArgs eventargs)
        {
            Console.WriteLine(eventargs.Client.ClientID + " recieved message " + eventargs.Command.ToString());
        }
    }
}
