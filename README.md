# NekoIOLabs-Tcp-Communication
## General Info

This is a library I made so that people don't have to rewrite common TCP communication classes for server or client. This library has a genreal implementation of a TCP Server and client.
Through the IProtocolParser Interface you are able to add any kind of protocol and get the parsed messages back from the library. As standard I'll release the library with Websocket support since there is no good websocket server provided for .net core outside of asp.net as far as i know. You also have the ILog

## Features Progression
- [x] Bare TCP Communication 
- [ ] TLS Communication
- [ ] UDP Communication
- [x] Raw TCP Protocol Parser
- [ ] WebSocket Protocol Parser
- [x] Custom Protocol Parsers Through the IProtocolParser Interface
- [x] Custom Logging Interface
- [ ] Multiple Unit tests

## Basic Setup

  It is realy simple to include this into your project. The only thing you need is to keep a var who hold the NekoIOLabsServer object 
  and then plug in the parser you want for the server or keep the raw data.
  
 ### 1. Using the default parser and logger
 ```csharp
//step one add the using statement atop;
using NekoIOLabsTcpCommunication.Server.Models;

//step two construct the server with a specific ip or any and a custom port of 8080 here
NekoIOLabsServer server = new NekoIOLabsServer(IPAddress.Any,NEKOIOLABS_COMMUNICATION_TYPE.TCP,8080);

//step three register to the two events that the server emit
server.OnMessageDecoded += Server_OnMessageDecoded;
server.OnClientStatusChanged += Server_OnClientStatusChanged;

//handle these events as you see fit
private static void Server_OnClientStatusChanged(NekoIOLabsTcpCommunication.Server.Events.ClientStateEventArgs args)
        {
           //handle client status you get the client id and a enum with the current status
        }

private static void Server_OnMessageDecoded(NekoIOLabsTcpCommunication.Server.Events.MessageParsedEventArgs eventargs)
        {
            //handle the decoded message you recieve from the protocol parser this will be an ICommand
            Console.WriteLine(eventargs.Client.ClientID + " recieved message " + eventargs.Command.ToString());
        }

// start the server it will automaticly run on a seperated thread
server.Start();

//to stop the server simply call
server.Stop();
```

 

 

## Using Existing Parser
todo

## Writing Custom Parser

todo

## Using an Existing Logger

## Writing a Custom logging