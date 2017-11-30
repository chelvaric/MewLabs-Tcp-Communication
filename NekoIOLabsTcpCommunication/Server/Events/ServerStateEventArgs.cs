using NekoIOLabsTcpCommunication.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NekoIOLabsTcpCommunication.Server.Events
{
    public class ServerStateEventArgs
    {

        private SERVER_STATE _state;

        public SERVER_STATE State
        {
            get { return _state; }
            set { _state = value; }
        }


        public ServerStateEventArgs(SERVER_STATE state)
        {
            _state = state;
        }

    }
}
