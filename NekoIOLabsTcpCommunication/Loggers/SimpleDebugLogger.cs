using NekoIOLabsTcpCommunication.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NekoIOLabsTcpCommunication.Loggers
{
    class SimpleDebugLogger : ILogger
    {
        public void LogMessage(string LogMessage, LOG_TYPE typeOfLog)
        {
            string type = "INFO";
            switch (typeOfLog)
            {
                case LOG_TYPE.INFO:
                    type = "INFO";
                    break;
                case LOG_TYPE.DEBUG:
                    type = "DEBUG";
                    break;
                case LOG_TYPE.ERROR:
                    type = "ERROR";
                    break;
                case LOG_TYPE.CRITICAL:
                    type = "CRITICAL";
                    break;
              
            }


            Debug.WriteLine(type + ": " + DateTime.Now + " : " + LogMessage);
        }
    }
}
