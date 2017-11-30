using MewLabTcpCommunication.Interfaces;
using MewLabTcpCommunication.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace MewLabTcpCommunication.Server.Events
{
    public delegate void OnMessagePargedEventHandler(MessageParsedEventArgs eventargs);
    public class MessageParsedEventArgs
    {

        public MewLabsConnectedClient Client{get;set;}
        public IMessage Command { get; set; }

       

    }
}
