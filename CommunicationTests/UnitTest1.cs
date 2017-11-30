using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommunicationTests
{
    [TestClass]
    public class CommunicationTest
    {
        [TestMethod]
        public void TcpCommunicationTest()
        {
            var server = new MewLabTcpCommunication.Server.Models.MewLabsServer(IPAddress.Any, MewLabTcpCommunication.Server.Models.MEWLABS_COMMUNICATION_TYPE.TCP, 54001);
           

                Task.Run(async () =>
                {
                    var serverTask = server.Start();

                    var tasks = new List<Task>();

                    for (var i = 0; i < 5; ++i)
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            var response = new byte[1024];

                            using (var client = new TcpClient())
                            {
                                client.Connect("127.0.0.1", 54001);

                                using (var stream = client.GetStream())
                                {
                                    var request = Encoding.ASCII.GetBytes("Knock, knock...");
                                    stream.Write(request, 0, request.Length);
                                    

                                    //Assert.AreEqual("Who's there?", Encoding.ASCII.GetString(response).TrimEnd('\0'));
                                    Debug.WriteLine($"Who's there? Echo: " + Encoding.ASCII.GetString(response).TrimEnd('\0') + $" [{Thread.CurrentThread.ManagedThreadId}]");
                                }
                            }
                        }));
                    }
                    await serverTask;

                    //Assert.IsTrue(Task.WaitAll(tasks.ToArray(), 10000));
                    Debug.WriteLine($"IsTrue: " + Task.WaitAll(tasks.ToArray(), 10000));

                   
                });

            
        }
    }
}
