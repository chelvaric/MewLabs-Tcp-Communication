using System;
using System.Collections.Generic;
using System.Text;

namespace NekoIOLabsTcpCommunication.Interfaces
{
    public interface IMessage
    {
   
        
        byte[] Serialize();
    }
}
