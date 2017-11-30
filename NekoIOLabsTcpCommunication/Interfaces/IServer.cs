using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NekoIOLabsTcpCommunication.Server.Models;
using System.Threading.Tasks;
using NekoIOLabsTcpCommunication.Server.Events;

namespace NekoIOLabsTcpCommunication.Interfaces
{
    delegate void OnClientConnectedHandler(IServer server,ClientConnectedEventArgs args);
   internal interface IServer : IDisposable
    {

        NekoIOLabsServerState ServerState { get;  }

   

        event OnClientConnectedHandler OnClientConnected;

        int Port { get;  }

        string IpAddress { get; }

         Task StartListening();
        void CloseServer();

    }
}
