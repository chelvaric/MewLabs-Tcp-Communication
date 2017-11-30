using NekoIOLabsTcpCommunication.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NekoIOLabsTcpCommunication.Server.Events
{
   public class ClientStateEventArgs
    {

        private CLIENT_STATE _state;

        public CLIENT_STATE State
        {
            get { return _state; }
            set { _state = value; }
        }

        private NekoIOLabsConnectedClient _client;

        public NekoIOLabsConnectedClient Client
        {
            get { return _client; }
            set { _client = value; }
        }



        public ClientStateEventArgs(NekoIOLabsConnectedClient sender,CLIENT_STATE state)
        {
            _state = state;
        }
    }
}
