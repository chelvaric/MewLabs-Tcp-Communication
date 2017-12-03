using NekoIOLabsTcpCommunication.Interfaces;
using NekoIOLabsTcpCommunication.Loggers;
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


        private static ILogger _logger;

        public static ILogger Logger => _logger;


        #endregion
    

        //events 
        public event OnMessagePargedEventHandler OnMessageDecoded;
        public event OnClientEventHandler OnClientStatusChanged;

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
                //check if they plugged in a parsere other wise use default parser
                if(parser == null)
                {
                    _protocolParser = new RawProtocolParser();
                }
                else
                {
                    _protocolParser = parser;
                }

                //check if they plugged in a logger other wise use default logger
                if (Logger == null)
                {
                    if (logger == null)
                    {
                        _logger = new SimpleDebugLogger();
                    }
                    else
                    {
                        _logger = logger;
                    }

                 }
                // set up the communcation thread
                SetCommuncations(type, ip);
                //init client events from the communication and the client list
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

                //check if they plugged in a logger other wise use default logger
                if (Logger == null)
                {
                    if (logger == null)
                    {
                        _logger = new SimpleDebugLogger();
                    }
                    else
                    {
                        _logger = logger;
                    }

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

                NekoIOLabsServer.Logger?.LogMessage("client" + client.ClientID + " connected",LOG_TYPE.DEBUG);

                _connectedClients.Add(client);

                client.OnMessageDecoded += Client_OnMessageDecoded;
                client.OnClientEvent += Client_OnClientEvent;

                client.StartRecieve();
            }
        }

        private void Client_OnClientEvent(ClientStateEventArgs args)
        =>    OnClientStatusChanged?.Invoke(args);
        

        private void Client_OnMessageDecoded(MessageParsedEventArgs eventargs)
            => OnMessageDecoded?.Invoke(eventargs);
        

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

        /// <summary>
        /// Get a list of the currently connected Clients
        /// </summary>
        /// <returns></returns>
        public List<NekoIOLabsConnectedClient> GetAllConnectedClients()
        {
            lock (_connectedClients)
            {
                return _connectedClients;
            }
        }
        
        /// <summary>
        /// Get the Client object of an ID
        /// </summary>
        /// <param name="id">Client ID</param>
        /// <returns></returns>
        public NekoIOLabsConnectedClient GetClient(Guid id)
        {
            lock (_connectedClients)
            {
                if (id == null)
                    return null;

                return _connectedClients.SingleOrDefault(x=>x.ClientID == id);
            }
        }
        /// <summary>
        /// Give the command and client id to send a message to a client.
        /// </summary>
        /// <param name="id">Client Id</param>
        /// <param name="message">a object that implements IMessage</param>
        public void  SendToClient(Guid id,IMessage message)
        {
            var client = GetClient(id);
            if(client != null)
            {
                client.SendData(message);
            }
        }

        /// <summary>
        /// Disconnect a client socket by its ID
        /// </summary>
        /// <param name="id">The Client ID</param>
        /// <returns>flase or true if the client still excist</returns>
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
        /// <summary>
        /// Disconnect all the clients
        /// </summary>
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
