using MewLabTcpCommunication.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using MewLabTcpCommunication.Server.Models;
using MewLabTcpCommunication.RawParser.commands;

namespace MewLabTcpCommunication.RawParser
{
    class RawProtocolParser : IProtocolParser
    {
        public List<MEWLABS_COMMUNICATION_TYPE> AcceptedCommTypes =>new List<MEWLABS_COMMUNICATION_TYPE>() {MEWLABS_COMMUNICATION_TYPE.TCP,MEWLABS_COMMUNICATION_TYPE.TLS,MEWLABS_COMMUNICATION_TYPE.UDP };

        public IMessage ParseData(byte[] data)
        {
            RawCommand cmd = new RawCommand();
            cmd.Data = data;

            return cmd;
                
         }
    }
}
