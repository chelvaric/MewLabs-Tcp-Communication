using MewLabTcpCommunication.Server.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MewLabTcpCommunication.Server.Models
{


    public enum CLIENT_STATE
    {
        CLOSED =0,
        CONNECTING = 1,
        CONNECTED = 2,
        FAULTED = 3,
        CLOSING = 4
    }

    public delegate void OnClientEventHandler(ClientStateEventArgs args);

   
}
