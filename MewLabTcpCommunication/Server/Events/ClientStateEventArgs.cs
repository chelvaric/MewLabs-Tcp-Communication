using MewLabTcpCommunication.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MewLabTcpCommunication.Server.Events
{
   public class ClientStateEventArgs
    {

        private CLIENT_STATE _state;

        public CLIENT_STATE State
        {
            get { return _state; }
            set { _state = value; }
        }


        public ClientStateEventArgs(MewLabsConnectedClient sender,CLIENT_STATE state)
        {
            _state = state;
        }
    }
}
