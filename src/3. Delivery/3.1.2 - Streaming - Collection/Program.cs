using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery;
using Refinitiv.DataPlatform.Delivery.Stream;

// **********************************************************************************************************************
// 3.1.2 - Streaming - Collection
// The following example demonstrates the use of retrieving multiple instruments and waiting for the entire collection
// to be retrieved using the API.  
//
// Note: To configure settings for your environment, visit the following files:
//      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
//      2. Configuration.Credentials to define your login credentials for the specified access channel.
// **********************************************************************************************************************
namespace Collection_Request
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the platform session.
            ISession session = Configuration.Sessions.GetSession();

            // Open the session
            if ( session.Open() == Session.State.Opened )
            {
                // *******************************************************************************************************************************
                // Requesting for multiple instruments.
                // The ItemStream interface supports the ability to request/stream a single item.  The following code segment utilizes the power
                // of the .NET asynchronous libraries to send a collection of requests and monitor the whole collection for completion.
                // *******************************************************************************************************************************
                List<Task<Stream.State>> tasks = new List<Task<Stream.State>>();

                // First, prepare our item stream details including the fields of interest and where to capture events...
                var itemParams = new ItemStream.Params().Session(session).WithFields("DSPLY_NAME", "BID", "ASK")
                                                                         .OnRefresh((s, msg) => DumpMsg(msg))
                                                                         .OnUpdate((s, msg) => DumpMsg(msg))
                                                                         .OnStatus((s, msg) => Console.WriteLine(msg));

                // Next, iterate through the collection of items, applying each to our parameters specification.  Send each request asynchronously...
                foreach (var item in new[] { "EUR=", "GBP=", "CAD=" })
                {
                    // Create our stream
                    IStream stream = DeliveryFactory.CreateStream(itemParams.Name(item));

                    // Open the stream asynchronously and keep track of the task
                    tasks.Add(stream.OpenAsync());
                }

                // Monitor the collection for completion.  We are intentionally blocking here waiting for the whole collection to complete.
                Task.WhenAll(tasks).GetAwaiter().GetResult();
                Console.WriteLine("\nInitial response for all instruments complete.  Updates will follow based on changes in the market...");

                // Wait for updates...
                Console.ReadKey();
            }
        }

        // Based on market data events, reach into the message and pull out the fields of interest for our display.
        private static void DumpMsg(JObject msg)
        {
            // The MarketPrice message contains a nested structure containing the fields included within the market data event.
            var fields = msg["Fields"];

            if ( fields != null )
            {
                // Our quote fields
                double bid = (double)fields["BID"];
                double ask = (double)fields["ASK"];

                // Pull out the specific item that is being updated
                string item = (string)msg["Key"]?["Name"] ?? "<unknown>";

                // Display the quote for the asset we're watching
                Console.WriteLine($"{ DateTime.Now.ToString("HH:mm:ss")}: {item} ({bid,6}/{ask,6}) - {fields["DSPLY_NAME"]}");
            }
        }
    }
}
