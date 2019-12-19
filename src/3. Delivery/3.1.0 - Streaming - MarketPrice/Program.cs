using System;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery;
using Refinitiv.DataPlatform.Delivery.Stream;

// **********************************************************************************************************************
// 3.1.0 - Streaming - MarketPrice
// The following example demonstrates a basic use case of the API to retrieve streaming content from the platform.
// MarketPrice refers to level 1 content such as trades and quotes.
//
// Note: To configure settings for your environment, visit the following files:
//      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
//      2. Configuration.Credentials to define your login credentials for the specified access channel.
// **********************************************************************************************************************
namespace MarketPrice
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a session to connect into the platform.
            ISession session = Configuration.Sessions.GetSession();

            // Open the session
            session.Open();

            // Define a stream to retrieve level 1 content...
            using (IStream stream = DeliveryFactory.CreateStream(
                                                new ItemStream.Params().Session(session)
                                                                       .Name("EUR=")
                                                                       .OnRefresh((s, msg) => Console.WriteLine(msg))
                                                                       .OnUpdate((s, msg) => Console.WriteLine(msg))
                                                                       .OnStatus((s, msg) => Console.WriteLine(msg))))
            {
                // Open the stream...
                stream.Open();

                // Wait for data to come in then hit any key to close the stream...
                Console.ReadKey();
            }
        }
    }
}


