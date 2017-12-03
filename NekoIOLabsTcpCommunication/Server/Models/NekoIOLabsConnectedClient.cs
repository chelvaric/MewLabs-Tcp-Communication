using NekoIOLabsTcpCommunication.Interfaces;
using NekoIOLabsTcpCommunication.Server.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NekoIOLabsTcpCommunication.Server.Models
{
   public class NekoIOLabsConnectedClient : IDisposable
    {
        //the parent server
        NekoIOLabsServer _server;

  
     
        //the accepted tcpClient
        TcpClient _client;

        //cancel mechanism when closing the connection
        CancellationToken _token;
        CancellationTokenSource source = new CancellationTokenSource();

        public event OnMessagePargedEventHandler OnMessageDecoded;

        Thread _recieveThread;

        private Guid _clientID;
        public Guid ClientID => _clientID;


        public event OnClientEventHandler OnClientEvent;

        private CLIENT_STATE _state;

        public CLIENT_STATE State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    HandleEvent();
                }
            }
        }

        public void HandleEvent()
        {
            OnClientEvent?.Invoke(new Events.ClientStateEventArgs(this, _state));
        }

        internal NekoIOLabsConnectedClient(Guid clientId, NekoIOLabsServer server, TcpClient dataStream)
        {
            _server = server;
            _clientID = clientId;

            _client = dataStream;
            _recieveThread  = new Thread(async () => { await StartRecieveData(); });

          
        }


        internal void StartRecieve()
        {
            _recieveThread.Start();
        }


        public async Task StartRecieveData()
        {
            Stream sr = _client.GetStream();
            byte[] buffer = new byte[1024];
            try
            {
                while (!_token.IsCancellationRequested)
                {
                    int dataRecieved = await sr.ReadAsync(buffer, 0, 1024,_token);
                   
                    if(dataRecieved > 0)
                    {
                        NekoIOLabsServer.Logger?.LogMessage( "MessageRecieved " + _clientID +" " + ASCIIEncoding.ASCII.GetString(buffer),LOG_TYPE.INFO);
                       IMessage message = _server?.ProtocolParser?.ParseData(buffer);
                        if(message != null)
                        {
                            OnMessageDecoded?.Invoke(new MessageParsedEventArgs() { Client = this, Command = message });
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                State = CLIENT_STATE.FAULTED;
                NekoIOLabsServer.Logger?.LogMessage(ex.Message,LOG_TYPE.ERROR);
            }
        }


        public async void SendData(IMessage data)
        {
            try
            {
                if (_client.Connected)
                {

                    byte[] bytedata = data.Serialize();
                    if (bytedata != null)
                    {
                        await _client.GetStream().WriteAsync(bytedata, 0, bytedata.Length,_token);
                    }
                }
            }
            catch(ArgumentOutOfRangeException ex)
            {
                State = CLIENT_STATE.FAULTED;
                NekoIOLabsServer.Logger?.LogMessage("tried to write bytes outside of the byte array to the client",LOG_TYPE.ERROR);
            }
            catch(ObjectDisposedException ex)
            {
                State = CLIENT_STATE.FAULTED;
                NekoIOLabsServer.Logger?.LogMessage("tried to write bytes outside of the byte array to the client",LOG_TYPE.ERROR);
            }
        }

        public void Disconnect()
        {

            source.Cancel();
            _recieveThread.Join();
            if(_client != null)
            {
                _client.Close();
                _client.Dispose();
                State = CLIENT_STATE.CLOSED;
            }
            
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
