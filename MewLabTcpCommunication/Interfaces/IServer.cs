using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MewLabTcpCommunication.Server.Models;
using System.Threading.Tasks;
using MewLabTcpCommunication.Server.Events;

namespace MewLabTcpCommunication.Interfaces
{
    delegate void OnClientConnectedHandler(IServer server,ClientConnectedEventArgs args);
   internal interface IServer : IDisposable
    {

        MewLabsServerState ServerState { get;  }

   

        event OnClientConnectedHandler OnClientConnected;

        int Port { get;  }

        string IpAddress { get; }

         Task StartListening();
        void CloseServer();

    }
}
