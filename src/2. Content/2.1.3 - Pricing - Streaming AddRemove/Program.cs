using Refinitiv.DataPlatform.Content;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery.Stream;
using System;

// **********************************************************************************************************************
// 2.1.3 - Pricing - Streaming AddRemove
// The following example demonstrates how add to or remove items from your streaming cache.  Items added will be
// automatically opened if the stream is already opened.
//
// Note: To configure settings for your environment, visit the following files:
//      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
//      2. Configuration.Credentials to define your login credentials for the specified access channel.
// **********************************************************************************************************************
namespace Streaming_Items
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a session into the platform...
            ISession session = Configuration.Sessions.GetSession();

            // Open the session
            session.Open();

            // Create a streaming price interface for a list of instruments and specify lambda expressions to capture real-time updates
            using (var stream = Pricing.CreateStreamingPrices(new StreamingPrices.Params().Universe("EUR=", "CAD=", "GBP=")
                                                                                          .WithFields("DSPLY_NAME", "BID", "ASK")
                                                                                          .OnStatus((o, item, status) => Console.WriteLine(status))))
            {
                if ( stream.Open() == Stream.State.Opened )
                {

                    // Dump the cache to show the current items we're watching
                    DumpCache(stream);

                    // Add 2 new currencies...
                    stream.AddItems("JPY=", "MXN=");

                    // Dump cache again...
                    DumpCache(stream);

                    // Remove 2 different currencies...
                    stream.RemoveItems("CAD=", "GBP=");

                    // Final dump
                    DumpCache(stream);

                    // Close streams
                    Console.WriteLine("\nClosing open streams...");
                }
            }
        }

        private static void DumpCache(IStreamingPrices stream)
        {
            Console.WriteLine("\n*************************************Current cached items**********************************");

            foreach( var entry in stream)
                Console.WriteLine($"{entry.Key}: {entry.Value["DSPLY_NAME"]}");
        }
    }
}
