using NekoIOLabsTcpCommunication.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NekoIOLabsTcpCommunication.Interfaces
{
    public interface IProtocolParser
    {


        List<NEKOIOLABS_COMMUNICATION_TYPE> AcceptedCommTypes { get;  }

        

       IMessage ParseData(byte[] data);
        

    }
}
