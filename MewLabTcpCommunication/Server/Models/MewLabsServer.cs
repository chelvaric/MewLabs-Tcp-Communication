using MewLabTcpCommunication.Interfaces;
using MewLabTcpCommunication.RawParser;
using MewLabTcpCommunication.Server.Communication;
using MewLabTcpCommunication.Server.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MewLabTcpCommunication.Server.Models
{
   public  class MewLabsServer: IDisposable
    {

        private MEWLABS_COMMUNICATION_TYPE __communicationType;


        public MEWLABS_COMMUNICATION_TYPE ComunicationType
        {
            get { return __communicationType; }
            
        }


        //lock object so its threadsafe
        object _lockConnectedClients = new object();
        List<MewLabsConnectedClient> _connectedClients;

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

        //The parser of the protocol that will be send
        private IProtocolParser _protocolParser;

        public IProtocolParser ProtocolParser => _protocolParser;

        public event OnMessagePargedEventHandler OnMessageDecoded;

        /// <summary>
        /// Construct a nEw MewLabs Tcp Server, will use default port 80 fro tcp and udp and port 443 for TLS
        /// </summary>
        /// <param name="ip">Tye IpAddress of the server</param>
        /// <param name="type">The type of communication the server uses (tcp,tls,udp)</param>
        /// <param name="parser">The parser that is needed to parse the protocol send over the Tcp Communication, NUll by default will use the RawProtocol Parser</param>
        public MewLabsServer(IPAddress ip,MEWLABS_COMMUNICATION_TYPE type,IProtocolParser parser = null)
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
                _connectedClients = new List<MewLabsConnectedClient>();
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
        public MewLabsServer(IPAddress ip, MEWLABS_COMMUNICATION_TYPE type,int port, IProtocolParser parser = null)
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
                _connectedClients = new List<MewLabsConnectedClient>();
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
                MewLabsConnectedClient client = new MewLabsConnectedClient(args.ClientID, this, args.ClientData);

                Debug.WriteLine("client" + client.ClientID + " connected");

                _connectedClients.Add(client);

                client.OnMessageDecoded += Client_OnMessageDecoded;

                client.StartRecieve();
            }
        }

        private void Client_OnMessageDecoded(MessageParsedEventArgs eventargs)
        {
            OnMessageDecoded?.Invoke(eventargs);
        }

        public void Stop()
        {
            DisconnectAll();
            serverCommunication?.CloseServer();
        }

        public async Task Start()
        {
            if (serverCommunication == null)
                return;
            else
            {
                await serverCommunication.StartListening();
                
            }
        }

        /// <summary>
        /// create the correct communication class
        /// </summary>
        /// <param name="type">The type of comunication that is used ex. TCP or TLS or UDP</param>
        private void SetCommuncations(MEWLABS_COMMUNICATION_TYPE type,IPAddress ip,int? port = null)
        {
            switch(type)
            {
                case MEWLABS_COMMUNICATION_TYPE.TCP:
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
          
        }


        public List<MewLabsConnectedClient> GetAllConnectedClients()
        {
            lock (_connectedClients)
            {
                return _connectedClients;
            }
        }

        public MewLabsConnectedClient GetClient(Guid id)
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
                    client.Disconnect();
                }
            }
        }
    }
}
