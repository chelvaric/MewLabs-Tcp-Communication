using NekoIOLabsTcpCommunication.Interfaces;
using NekoIOLabsTcpCommunication.RawParser;
using NekoIOLabsTcpCommunication.Server.Communication;
using NekoIOLabsTcpCommunication.Server.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NekoIOLabsTcpCommunication.Server.Models
{
   public  class NekoIOLabsServer: IDisposable
    {

        private NEKOIOLABS_COMMUNICATION_TYPE __communicationType;


        public NEKOIOLABS_COMMUNICATION_TYPE ComunicationType
        {
            get { return __communicationType; }
            
        }


        Thread _listenerThread;

        //lock object so its threadsafe
        object _lockConnectedClients = new object();
        List<NekoIOLabsConnectedClient> _connectedClients;

        public string IP
        {
            get
            {
                if (serverCommunication != null)
                    return serverCommunication.IpAddress;
                else
                    return "IP Address Not Set";
            }
        }

        /// <summary>
        /// returns -1 if the communication isn't established
        /// </summary>
        public int Port
        {
            get
            {
                if (serverCommunication != null)
                    return serverCommunication.Port;
                else
                    return  -1;
            }
        }

        //the interface with the type of communication that is Requested
        private IServer serverCommunication;

        #region the plugable interfaces

        //The parser of the protocol that will be send
        private IProtocolParser _protocolParser;

        public IProtocolParser ProtocolParser => _protocolParser;


        private ILogger _logger;

        public ILogger Logger => _logger;


        #endregion
    


        public event OnMessagePargedEventHandler OnMessageDecoded;

        /// <summary>
        /// Construct a nEw MewLabs Tcp Server, will use default port 80 fro tcp and udp and port 443 for TLS
        /// </summary>
        /// <param name="ip">Tye IpAddress of the server</param>
        /// <param name="type">The type of communication the server uses (tcp,tls,udp)</param>
        /// <param name="parser">The parser that is needed to parse the protocol send over the Tcp Communication, NUll by default will use the RawProtocol Parser</param>
        public NekoIOLabsServer(IPAddress ip,NEKOIOLABS_COMMUNICATION_TYPE type,IProtocolParser parser = null,ILogger logger = null)
        {
            try
            {
                if(parser == null)
                {
                    _protocolParser = new RawProtocolParser();
                }
                else
                {
                    _protocolParser = parser;
                }

                SetCommuncations(type, ip);
                _connectedClients = new List<NekoIOLabsConnectedClient>();
                serverCommunication.OnClientConnected += ServerCommunication_OnClientConnected;
            }
            catch(Exception ex)
            {
                throw new Exception("creation of server failed");
            }
        }

        /// <summary>
        /// Construct a nEw MewLabs Tcp Server
        /// </summary>
        /// <param name="ip">Tye IpAddress of the server</param>
        /// <param name="type">The type of communication the server uses (tcp,tls,udp)</param>
        /// <param name="port">The port the server listens on</param>
        /// <param name="parser">The parser that is needed to parse the protocol send over the Tcp Communication, NUll by default will use the RawProtocol Parser</param>
        public NekoIOLabsServer(IPAddress ip, NEKOIOLABS_COMMUNICATION_TYPE type,int port, IProtocolParser parser = null,ILogger logger = null)
        {
            try
            {
                if (parser == null)
                {
                    _protocolParser = new RawProtocolParser();
                }
                else
                {
                    _protocolParser = parser;
                }

                SetCommuncations(type, ip,port);
                _connectedClients = new List<NekoIOLabsConnectedClient>();
                serverCommunication.OnClientConnected += ServerCommunication_OnClientConnected; 
            }
            catch (Exception ex)
            {
                throw new Exception("creation of server failed");
            }
        }

        private void ServerCommunication_OnClientConnected(IServer server, Events.ClientConnectedEventArgs args)
        {
           lock(_lockConnectedClients)
            {
                NekoIOLabsConnectedClient client = new NekoIOLabsConnectedClient(args.ClientID, this, args.ClientData);

                Debug.WriteLine("client" + client.ClientID + " connected");

                _connectedClients.Add(client);

                client.OnMessageDecoded += Client_OnMessageDecoded;
                client.OnClientEvent += Client_OnClientEvent;

                client.StartRecieve();
            }
        }

        private void Client_OnClientEvent(ClientStateEventArgs args)
        {
            
        }

        private void Client_OnMessageDecoded(MessageParsedEventArgs eventargs)
        {
            OnMessageDecoded?.Invoke(eventargs);
        }

        public void Stop()
        {
            DisconnectAll();
            serverCommunication?.CloseServer();
            _listenerThread.Join();
        }

        public void Start()
        {
            if (serverCommunication == null)
                return;
            else
            {
                //start the thread that keeps the listener alive
                _listenerThread = new Thread(() => {  serverCommunication.StartListening().Wait(); });
                _listenerThread.Start();
            }
        }

        /// <summary>
        /// create the correct communication class
        /// </summary>
        /// <param name="type">The type of comunication that is used ex. TCP or TLS or UDP</param>
        private void SetCommuncations(NEKOIOLABS_COMMUNICATION_TYPE type,IPAddress ip,int? port = null)
        {
            switch(type)
            {
                case NEKOIOLABS_COMMUNICATION_TYPE.TCP:
                    {
                        if (port != null)
                        {
                            serverCommunication = new TcpCommunication(ip,(int)port);
                        }
                        else
                        {
                            serverCommunication = new TcpCommunication(ip);
                        }
                        break;
                    }
            }
        }

        public void Dispose()
        {
            DisconnectAll();
            serverCommunication?.Dispose();
            _listenerThread.Abort();
            _listenerThread.Join();

          
        }


        public List<NekoIOLabsConnectedClient> GetAllConnectedClients()
        {
            lock (_connectedClients)
            {
                return _connectedClients;
            }
        }

        public NekoIOLabsConnectedClient GetClient(Guid id)
        {
            lock (_connectedClients)
            {
                if (id == null)
                    return null;

                return _connectedClients.SingleOrDefault(x=>x.ClientID == id);
            }
        }

        public bool DisconnectClient(Guid id)
        {

            var client = GetClient(id);
            if(client == null)
            {
                return false;
            }
            else
            {
                client.OnClientEvent -= Client_OnClientEvent;
                client.OnMessageDecoded -= Client_OnMessageDecoded;
                client.Disconnect();
                return true;
            }

            
        }

        public void DisconnectAll()
        {
            lock(_lockConnectedClients)
            {
                foreach(var client in _connectedClients)
                {
                    client.OnClientEvent -= Client_OnClientEvent;
                    client.OnMessageDecoded -= Client_OnMessageDecoded;
                    client.Disconnect();
                }
            }
        }
    }
}
