using System;
using System.Collections.Generic;
using System.Text;

namespace NekoIOLabsTcpCommunication.Interfaces
{

    public enum LOG_TYPE
    {
        INFO,
        DEBUG,
        ERROR,
        CRITICAL
    }

    public interface ILogger
    {
        /// <summary>
        /// the method that will be called inside the dll so that you can capture it and save where you want
        /// </summary>
        /// <param name="LogMessage">the text of the error or log</param>
        /// <param name="typeOfLog">What type of log it is</param>
        void LogMessage(string LogMessage, LOG_TYPE typeOfLog);


    }
}
