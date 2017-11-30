using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace MewLabTcpCommunication.Server.Events
{
    public class ClientConnectedEventArgs
    {

        private Guid _clientId;

        public Guid ClientID
        {
            get { return _clientId; }
         
        }

        private TcpClient _clientData;

        public TcpClient ClientData
        {
            get { return _clientData; }
            set { _clientData = value; }
        }

        public ClientConnectedEventArgs(Guid client, TcpClient clientData)
        {
            _clientId = client;
            _clientData = clientData;
        }

    }
}
