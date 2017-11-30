using System;
using System.Collections.Generic;
using System.Text;

namespace MewLabTcpCommunication.Interfaces
{
    public interface IMessage
    {
   
        
        byte[] Serialize();
    }
}
