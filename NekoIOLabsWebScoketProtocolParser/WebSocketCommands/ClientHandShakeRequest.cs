using System;
using System.Collections.Generic;
using System.Text;
using NekoIOLabsTcpCommunication.Interfaces;

namespace MewLabsWebScoketProtocolParser.WebSocketCommands
{
    class ClientHandShakeRequest : IMessage
    {

        Dictionary<string, object> _headers;


        private bool _validRequest;

        public bool ValidRequest
        {
            get { return _validRequest; }
            set { _validRequest = value; }
        }



        public ClientHandShakeRequest(Dictionary<string,object> headers)
        {
            _headers = headers;
        }

        public byte[] Serialize()
        {
            throw new NotImplementedException();
        }

        public void InitRequest()
        {
            //check if the version is atleast 1.1 or higher
            if((int)_headers["MajorVersion"] >=1 && (int)_headers["MinorVersion"] >= 1 )
            {
                //find the host
                if(_headers.ContainsKey("Host"))
                {
                    WebSocketHost = (string)_headers["Host"];
                }
                //search for the Upgrade Header THAT MUST BE IN THE REQUEST
                if (_headers.ContainsKey("Upgrade"))
                {
                    if(!((string)_headers["Upgrade"] == "websocket"))
                    {
                        _validRequest = false;
                    }

                }
                else
                {
                    _validRequest = false;
                }
                //search for the Connection Header THAT MUST BE IN THE REQUEST
                if(_headers.ContainsKey("Connection"))
                {
                    if (!((string)_headers["Connection"] == "Upgrade"))
                    {
                        _validRequest = false;
                    }
                }
                else
                {
                    _validRequest =  false;
                }

                //search for the Sec-WebSocket-Key Header THAT MUST BE IN THE REQUEST
                if (_headers.ContainsKey("Sec-WebSocket-Key"))
                {
                    SecureWebSocketKey = (string)_headers["Sec-WebSocket-Key"];

                }
                else
                {
                    _validRequest = false;
                }

                if (_headers.ContainsKey("Sec-WebSocket-Version"))
                {
                    if (int.TryParse((string)_headers["Sec-WebSocket-Version"],out int version))
                    {
                        WebSocketVersion = version;
                    }
                    else
                    {
                        _validRequest = false;
                    }
                }
                else
                {
                    _validRequest = false;
                }


                _validRequest = true;

            }
            else
            {
                _validRequest = false;
            }


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
