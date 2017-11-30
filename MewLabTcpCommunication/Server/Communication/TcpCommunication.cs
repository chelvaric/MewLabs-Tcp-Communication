using MewLabTcpCommunication.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using MewLabTcpCommunication.Server.Models;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MewLabTcpCommunication.Server.Communication
{
    class TcpCommunication : IServer
    {
        private MewLabsServerState _serverState; 
        public MewLabsServerState ServerState { get => _serverState;  }

        private int _port;
        public int Port => _port;

        private IPAddress _ip;
        public string IpAddress => _ip?.ToString();


        //hidden prop
        private TcpListener _listener;
        private CancellationToken _token;
        private CancellationTokenSource _cancellationTokenSource;

        public event OnClientConnectedHandler OnClientConnected;

        public TcpCommunication(IPAddress ipAddress)
        {
            _ip = ipAddress;

            _port = 8900;

            _serverState = new MewLabsServerState();
        }

        public TcpCommunication(IPAddress ipAddress, int port)
        {
            _ip = ipAddress;

            if(port < 80)
            {
                throw new ArgumentOutOfRangeException("port");
            }
            _port = port;

            _serverState = new MewLabsServerState();    
        }

        public void Dispose()
        {
            this.CloseServer();
        }

       

        public async Task StartListening()
        {

            CancellationToken? token = null;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token ?? new CancellationToken());

            _token = _cancellationTokenSource.Token;

            IPEndPoint endPoint = new IPEndPoint(_ip, _port);
            _listener = new TcpListener(endPoint);

            await ListenThread();
        }

        public void CloseServer()
        {
            _cancellationTokenSource.Cancel();
        }

        private async Task ListenThread()
        {
            try
            {
                //make the endpoint to listen on
              
                _listener.Start();
                ServerState.State = SERVER_STATE.LISTENING;

                //wait for each client
               while(!_token.IsCancellationRequested)
                {
                    await Task.Run(async () =>
                    {
                        //if there is a client waiting to be connected , we connect it async
                        if (_listener.Pending())
                        {
                            var result = await _listener.AcceptTcpClientAsync();

                            Guid clientID = Guid.NewGuid();

                            Console.WriteLine("client with id: " + clientID + " has connected");
                            
                            OnClientConnected?.Invoke(this, new Events.ClientConnectedEventArgs(clientID, result));

                        }

                    },_token);
                }

                
             
            }
            catch(Exception ex)
            {
                ServerState.State = SERVER_STATE.ERROR;
            }
            finally
            {
                _listener.Stop();
                ServerState.State = SERVER_STATE.CLOSED;
            }

        }


    }
}
