using Newtonsoft.Json.Linq;
using Refinitiv.DataPlatform.Content.Pricing;
using Refinitiv.DataPlatform.Core;
using System;

namespace _2._2._03_Pricing_StreamingEvents
{
    // **********************************************************************************************************************
    // 2.2.03-Pricing-StreamingEvents
    // The following example demonstrates how to open a streaming list of items using the Pricing interface and capture 
    // real-time events such as refreshes, updates and status messages generated from the platform.  To support this, 
    // the interface supports the definition of lambda expressions to capture events.
    //
    // Note: To configure settings for your environment, visit the following files within the .Solutions folder:
    //      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
    //      2. Configuration.Credentials to define your login credentials for the specified access channel.
    // **********************************************************************************************************************
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Create a session into the platform...
                using (ISession session = Configuration.Sessions.GetSession())
                {
                    // Open the session
                    session.Open();

                    // Create a streaming price interface for a list of instruments and specify lambda expressions to capture real-time updates
                    using (var stream = StreamingPrices.Definition("EUR=", "CAD=", "USD=").Fields("DSPLY_NAME", "BID", "ASK")
                                                                                          .OnRefresh((o, item, refresh) => Console.WriteLine(refresh))
                                                                                          .OnUpdate((o, item, update) => DisplayUpdate(item, update))
                                                                                          .OnStatus((o, item, status) => Console.WriteLine(status)))
                    {
                        stream.Open();

                        // Pause on the main thread while updates come in.  Wait for a key press to exit.
                        Console.WriteLine("Streaming updates.  Press any key to stop...");
                        Console.ReadKey();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }

        // Based on market data events, reach into the message and pull out the fields of interest for our display.
        private static void DisplayUpdate(string item, JObject fields)
        {
            // Display the quote for the asset we're watching
            Console.WriteLine($"{ DateTime.Now.ToString("HH:mm:ss")}: {item} ({fields["BID"],6}/{fields["ASK"],6}) - {fields["DSPLY_NAME"]}");
        }
    }
}
