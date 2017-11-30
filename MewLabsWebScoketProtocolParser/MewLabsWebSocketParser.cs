using System;
using System.Collections.Generic;
using System.Text;
using MewLabTcpCommunication.Interfaces;
using MewLabTcpCommunication.Server.Models;
using System.Text.RegularExpressions;
using MewLabsWebScoketProtocolParser.WebSocketCommands;

namespace MewLabsWebScoketProtocolParser
{
   public class MewLabsWebSocketParser : IProtocolParser
    {
        public List<MEWLABS_COMMUNICATION_TYPE> AcceptedCommTypes => new List<MEWLABS_COMMUNICATION_TYPE>() { MEWLABS_COMMUNICATION_TYPE.TCP, MEWLABS_COMMUNICATION_TYPE.TLS };

        public IMessage ParseData(byte[] data)
        {
            String message = Encoding.UTF8.GetString(data);

            if (Regex.IsMatch(message, "^GET"))
            {

                string[] parts = message.Split('\n');


                return new ClientHandShakeRequest();

            }
            else
            {
                return null;
            }

        }
    }
}
