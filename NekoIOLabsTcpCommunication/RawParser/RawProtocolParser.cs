using NekoIOLabsTcpCommunication.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using NekoIOLabsTcpCommunication.Server.Models;
using NekoIOLabsTcpCommunication.RawParser.commands;

namespace NekoIOLabsTcpCommunication.RawParser
{
    class RawProtocolParser : IProtocolParser
    {
        public List<NEKOIOLABS_COMMUNICATION_TYPE> AcceptedCommTypes =>new List<NEKOIOLABS_COMMUNICATION_TYPE>() {NEKOIOLABS_COMMUNICATION_TYPE.TCP,NEKOIOLABS_COMMUNICATION_TYPE.TLS,NEKOIOLABS_COMMUNICATION_TYPE.UDP };

        public IMessage ParseData(byte[] data)
        {
            RawCommand cmd = new RawCommand();
            cmd.Data = data;

            return cmd;
                
         }
    }
}
