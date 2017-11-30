using System;
using System.Collections.Generic;
using System.Text;
using NekoIOLabsTcpCommunication.Interfaces;

namespace MewLabsWebScoketProtocolParser.WebSocketCommands
{
    class ClientHandShakeRequest : IMessage
    {
        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }



        private string _secWebSocketKey;

        public string SecureWebSocketKey
        {
            get { return _secWebSocketKey; }
            set { _secWebSocketKey = value; }
        }

        private int _webSocketVersion;

        public int WebSocketVersion
        {
            get { return _webSocketVersion; }
            set { _webSocketVersion = value; }
        }

        private string _webSocketHost;

        public string WebSocketHost
        {
            get { return _webSocketHost; }
            set { _webSocketHost = value; }
        }




    }
}
