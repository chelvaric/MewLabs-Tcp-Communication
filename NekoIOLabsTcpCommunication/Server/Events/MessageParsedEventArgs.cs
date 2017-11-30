using NekoIOLabsTcpCommunication.Interfaces;
using NekoIOLabsTcpCommunication.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace NekoIOLabsTcpCommunication.Server.Events
{
    public delegate void OnMessagePargedEventHandler(MessageParsedEventArgs eventargs);
    public class MessageParsedEventArgs
    {

        public NekoIOLabsConnectedClient Client{get;set;}
        public IMessage Command { get; set; }

       

    }
}
