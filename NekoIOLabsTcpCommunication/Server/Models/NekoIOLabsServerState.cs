using NekoIOLabsTcpCommunication.Server.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace NekoIOLabsTcpCommunication.Server.Models
{
    public enum SERVER_STATE
    {
        LISTENING = 0,
        ERROR =1,
        CLOSED = 2
    }
    public class NekoIOLabsServerState
    {
        //events of Server State
        public delegate void ServerStateChangedHandler(ServerStateEventArgs args);

        public event ServerStateChangedHandler OnServerListening;
        public event ServerStateChangedHandler OnServerError;
        public event ServerStateChangedHandler OnServerClosed;
        //pops
        private SERVER_STATE _state;

        public SERVER_STATE State {
            get => _state;
            set  {
                if (_state != value)
                {
                    _state = value;
                    HandleEvent();
                }
            }
                }

        //methods
        //send the event out needed for this state
        void HandleEvent()
        {
            switch(State)
            {
                case SERVER_STATE.LISTENING: OnServerListening?.Invoke(new ServerStateEventArgs(_state)); break;
                case SERVER_STATE.CLOSED: OnServerClosed?.Invoke(new ServerStateEventArgs(_state)); break;
                case SERVER_STATE.ERROR: OnServerError?.Invoke(new ServerStateEventArgs(_state)); break;
            }
        }



    }
}
