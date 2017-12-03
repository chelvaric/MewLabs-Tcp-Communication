using System;
using System.Collections.Generic;
using System.Text;
using NekoIOLabsTcpCommunication.Interfaces;
using NekoIOLabsTcpCommunication.Server.Models;
using System.Text.RegularExpressions;
using MewLabsWebScoketProtocolParser.WebSocketCommands;
using NekoIOLabsWebScoketProtocolParser;

namespace MewLabsWebScoketProtocolParser
{
   public class NekoIOLabsWebSocketParser : IProtocolParser
    {
        public List<NEKOIOLABS_COMMUNICATION_TYPE> AcceptedCommTypes => new List<NEKOIOLABS_COMMUNICATION_TYPE>() { NEKOIOLABS_COMMUNICATION_TYPE.TCP, NEKOIOLABS_COMMUNICATION_TYPE.TLS };

        public IMessage ParseData(byte[] data)
        {
            String message = Encoding.UTF8.GetString(data);

            if (Regex.IsMatch(message, "^GET"))
            {

                try
                {
                  var headers =  HttpParser.GetHeaders(message);
                    ClientHandShakeRequest request =  new ClientHandShakeRequest(headers);
                    request.InitRequest();
                    
                        return request;
                    
                    
                }
                catch(FormatException ex)
                {
                    return null;
                }
              

            }
            else
            {
                return null;
            }

        }
    }
}
