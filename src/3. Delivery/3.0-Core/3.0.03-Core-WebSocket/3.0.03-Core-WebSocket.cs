using Newtonsoft.Json.Linq;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery;
using Refinitiv.DataPlatform.Delivery.Stream;
using System;

namespace _3._0._03_Core_WebSocket
{
    // **********************************************************************************************************************
    // 3.0.03-Core-WebSocket
    // The following example demonstrates how to register your own WebSocket implementation within the library. Utilizing the 
    // WebSocketSharp NuGet package, the 'WebSocketSharpImpl.cs' module provides the required interfaces to integrate within
    // the RDP Library for .Net.  In addition, this example also demonstrates how to select one of the built-in WebSocket 
    // implementations.
    //
    // Note: To configure settings for your environment, visit the following files within the .Solutions folder:
    //      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
    //      2. Configuration.Credentials to define your login credentials for the specified access channel.
    // **********************************************************************************************************************
    class Program
    {
        static void Main(string[] _)
        {
            try
            {
                // Choose a WebSocket Implementation
                Console.WriteLine("\nChoose a WebSocket Implementation:");
                Console.WriteLine("\t1 - ClientWebSocket (Microsoft implementation)");
                Console.WriteLine("\t2 - WebSocket4Net (originated from SuperWebSocket)");
                Console.WriteLine("\t3 - WebSocketSharp");

                Console.Write("\t==> ");
                string input = Console.ReadLine();
                int ver;
                if (int.TryParse(input, out ver))
                {
                    switch (ver)
                    {
                        case 1:
                            // The default in the library
                            break;
                        case 2:
                            // How to specify a version available within the library
                            DeliveryFactory.RegisterWebSocket(DeliveryFactory.WebSocketImpl.WebSocket4Net);
                            break;
                        case 3:
                            // Override with your own custom implementation
                            DeliveryFactory.RegisterWebSocketImpl = () => new WebSocketSharpImpl();
                            break;
                        default:
                            return;
                    }

                    using (ISession session = Configuration.Sessions.GetSession(false))
                    {
                        if (session.Open() == Session.State.Opened)
                            TestStreaming();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }

        private static void TestStreaming()
        {
            string item1 = "EUR=";
            string item2 = "CAD=";

            try
            {
                IStream stream1 = DeliveryFactory.CreateStream(
                                                    new ItemStream.Params().Session(Session.DefaultSession)
                                                                           .Name(item1)
                                                                           .OnRefresh((s, msg) => Console.WriteLine($"{DateTime.Now}:{msg}"))
                                                                           .OnUpdate((s, msg) => DumpMsg(msg))
                                                                           .OnStatus((s, msg) => Console.WriteLine($"{DateTime.Now} => Status1: {msg}"))
                                                                           .OnError((s, msg) => Console.WriteLine($"Stream1 error: {DateTime.Now}:{msg}")));
                if (stream1.Open() != Stream.State.Opened)
                {
                    Console.WriteLine($"Stream did not open: {stream1.OpenState}");
                }

                IStream stream2 = DeliveryFactory.CreateStream(
                                        new ItemStream.Params().Session(Session.DefaultSession)
                                                               .Name(item2)
                                                               .OnRefresh((s, msg) => Console.WriteLine($"{DateTime.Now}:{msg}"))
                                                               .OnUpdate((s, msg) => DumpMsg(msg))
                                                               .OnStatus((s, msg) => Console.WriteLine($"{DateTime.Now} => Status2: {msg}"))
                                                               .OnError((s, msg) => Console.WriteLine($"Stream2 error: {DateTime.Now}:{msg}")));
                stream2.Open();

                Console.ReadKey();
                stream1.Close();
                Console.WriteLine($"Stream {item1} has been closed.  Hit any key to close the {item2} stream...");
                Console.ReadKey();
                stream2.Close();
            }
            catch (PlatformNotSupportedException e)
            {
                Console.WriteLine($"\n******{e.Message} Choose an alternative WebSocket implementation.\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void DumpMsg(JObject msg)
        {
            JObject fields = (JObject)msg["Fields"];

            // Detect if we have a quote
            if (fields != null && fields["DSPLY_NAME"] != null)
            {
                double bid = (double)fields["BID"];
                double ask = (double)fields["ASK"];

                string item = (string)msg["Key"]["Name"] ?? "<unknown>";

                // Display the trade for the asset we're watching
                Console.WriteLine($"{ DateTime.Now:HH:mm:ss}: {item} ({bid}/{ask}) - {fields["DSPLY_NAME"]}");
            }
        }
    }
}
