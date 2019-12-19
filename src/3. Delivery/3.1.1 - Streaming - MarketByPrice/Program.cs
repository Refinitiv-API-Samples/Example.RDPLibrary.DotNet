using System;
using Newtonsoft.Json.Linq;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery;
using Refinitiv.DataPlatform.Delivery.Stream;


// **********************************************************************************************************************
// 3.1.1 - Streaming - MarketByPrice
// The following example demonstrates the use of retrieving a level 2 price book for a market data instrument using the 
// API. When requesting for large data items, such as a level 2 market price book, the streaming services will deliver 
// the initial refresh message in multiple segments.  We utilize the power of the NewtonSoft JSON libraries to merge the
// segments into 1 complete image.
//
// Note: To configure settings for your environment, visit the following files:
//      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
//      2. Configuration.Credentials to define your login credentials for the specified access channel.
// **********************************************************************************************************************
namespace MarketByPrice
{
    class Program
    {
        static void Main(string[] args)
        {
            // Complete image of the concatenation our MarketByPrice level 2 initial refreshes
            JObject image = new JObject();

            // Create a session into the platform.
            ISession session = Configuration.Sessions.GetSession();

            // Open the session
            session.Open();

            // Define a MarketByPrice stream specifying the Domain "MarketByPrice"
            // For each refresh, we merge the contents into the image object.  Once all refresh segments have arrived, the 
            // OnComplete is executed and the completed image details are presented for display.
            using (IStream stream = DeliveryFactory.CreateStream(
                                                new ItemStream.Params().Session(session)
                                                                       .Name("BB.TO")
                                                                       .WithDomain("MarketByPrice")
                                                                       .OnRefresh((s, msg) => image.Merge(msg))
                                                                       .OnComplete(s => DumpImage(image))
                                                                       .OnUpdate((s, msg) => DumpUpdate(msg))
                                                                       .OnStatus((s, msg) => Console.WriteLine(msg))))
            {
                // Open the stream...
                stream.Open();

                // Wait for data to come in then hit any key to close the stream...
                Console.ReadKey();
            }
        }

        // DumpImage
        // The DumpImage callback is invoked once the initial open for the stream has completed.  In our case, we
        // have requested for a level 2 book and thus we are notified when all segments have been delivered.
        private static void DumpImage(JObject image)
        {
            // The field "Map" contains the data we are interested in
            JToken data = image["Map"];

            if ( data != null )
            {
                // Level 2 items contain a Summary section.  Let's dump that here
                Console.WriteLine($"Price Book Header Summary:\n{data["Summary"]}\n");

                // Dump only the first order within the book.
                Console.WriteLine($"Price Book Entries:\n{data["Entries"][0]}\n\n...\n");

                // Instead of dumping the whole price book, we'll simply dump the number of orders outstanding in the book
                Console.WriteLine($"The price book contains {((JArray)data["Entries"])?.Count} entries.\n");
            }
        }

        // DumpUpdate
        // When an update comes in to the price book, we can expect the update to either "Add", "Update" or "Delete" entries
        // within the book.
        private static void DumpUpdate(JObject msg)
        {
            // The field "Entries" contains the data we are interested in
            JArray data = (JArray)msg["Map"]?["Entries"];

            if ( data != null )
            {
                // Iterate through our update - can contain multiple price book changes
                foreach (JToken item in data)
                {
                    JToken fields = item["Fields"];
                    if (fields != null)
                        Console.WriteLine($"{item["Action"]} => Side: {fields["ORDER_SIDE"]}, Price: {fields["ORDER_PRC"]}, " +
                                          $"Accumulated Volume: {fields["ACC_SIZE"]}, # of Orders: {fields["NO_ORD"]}");
                    else
                        Console.WriteLine($"{item["Action"]} => Key: {item["Key"]}");
                }
            }
        }
    }
}
