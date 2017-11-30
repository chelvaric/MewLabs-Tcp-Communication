using MewLabTcpCommunication.Server.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MewLabTcpCommunication.Interfaces
{
    public interface IProtocolParser
    {


        List<MEWLABS_COMMUNICATION_TYPE> AcceptedCommTypes { get;  }

        

       IMessage ParseData(byte[] data);
        

    }
}
