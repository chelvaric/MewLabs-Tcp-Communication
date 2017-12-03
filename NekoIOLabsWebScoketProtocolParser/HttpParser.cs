using System;
using System.Collections.Generic;
using System.Text;

namespace NekoIOLabsWebScoketProtocolParser
{
    internal static class HttpParser
    {

        public static Dictionary<string,object> GetHeaders(string message)
        {
            //initiliaze the dictionary
            Dictionary<string, object> InfoFromHttp = new Dictionary<string, object>();

            //split the lines with the newline sign of this enviroment
          string[] headers = message.Split(System.Environment.NewLine);
         
            //well loop over every part
            for(int i =0; i < headers.Length; i++)
            {
                string line = headers[i];

                //see if i is a header or the first line
                if (line.Contains(":"))
                {
                    string[] partsofLine = line.Split(' ');
                    if(partsofLine.Length > 1)
                    {
                       string name =partsofLine[0].TrimEnd(':');
                        InfoFromHttp.Add(name, partsofLine[1]);
                    }
                    else
                    {
                        //ignore this line we only want headers for the websocket protocol
                        continue;
                    }
                }
                else
                {
                    if (line.Contains("HTTP"))
                    {
                        //the first line is split with spaces ex. GET /chat HTTP/1.1 we only need the method and the http version
                        string[] partsofLine = line.Split(' ');
                        if (partsofLine.Length > 2)
                        {
                            InfoFromHttp.Add("Method", partsofLine[0]);

                            var (major, minor) = GetVersion(partsofLine[2]);
                            if (major == null || minor == null)
                                throw new FormatException("bad version in http message");

                            //place both of the numbers in the dictionary
                            InfoFromHttp.Add("MajorVersion", major);
                            InfoFromHttp.Add("MinorVersion", minor);


                        }
                        else
                        {
                            //this is a bad httpMessage
                            throw new FormatException("bad first line in http message");
                        }
                    }
                  }

                }

            return InfoFromHttp;


            }

        public static (int? Major,int? Minor) GetVersion(string versionPart)
        {
            //remove the HTTP/ and split the numbers 1.1 on the dot
            string version = versionPart.Remove(0, 5);
            string[] versionNumbers = version.Split('.');

            //its illigal to have leading zeros (1.04) but just in case this would be send we trim them
            string major = versionNumbers[0].TrimStart('0');
            string minor = versionNumbers[0].TrimStart('0');


            if (int.TryParse(major, out int majorVersionNumber))
            {
                if (int.TryParse(minor, out int minorVersionNumber))
                {
                    return (majorVersionNumber, minorVersionNumber);
                }
            }

            return (null,null);
        }

    }


      


    
}
