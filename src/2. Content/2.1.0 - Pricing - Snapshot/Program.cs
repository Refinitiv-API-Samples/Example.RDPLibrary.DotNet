using Refinitiv.DataPlatform.Content;
using Refinitiv.DataPlatform.Content.Data;
using Refinitiv.DataPlatform.Core;
using System;

// **********************************************************************************************************************
// 2.1.0 - Pricing - Snapshot
// The following example demonstrates a basic use case of the Pricing interface to retrieve a snapshot of prices from
// the platform.  The interface supports the ability to specify a list of items and the fields for each to retrieve. 
//
// Note: To configure settings for your environment, visit the following files:
//      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
//      2. Configuration.Credentials to define your login credentials for the specified access channel.
// **********************************************************************************************************************
namespace SnapshotPrice
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Create a session into the platform...
                ISession session = Configuration.Sessions.GetSession();

                // Open the session
                session.Open();

                // Specify a list of items and fields to retrieve a snapshot of prices from the platform
                var response = Pricing.GetSnapshot(new PricingSnapshot.Params().Universe("EUR=", "CAD=", "USD=")
                                                                               .WithFields("DSPLY_NAME", "BID", "ASK"));
                if (response.IsSuccess)
                {
                    // Print 1 field
                    Console.WriteLine($"\nEUR=[DSPLY_NAME]: {response.Data.Prices["EUR="]["DSPLY_NAME"]}");

                    // Print the contents of one item
                    Console.WriteLine($"\nEUR= contents: {response.Data.Prices["EUR="].Fields()}");

                    // Iterate through the cache print out specific fields for each entry
                    Console.WriteLine("\nIterate through the cache and display a couple of fields");
                    foreach (var item in response.Data.Prices)
                        Console.WriteLine($"Quote for item: {item.Key}\n{item.Value.Fields("BID", "ASK")}");
                }
                else
                    Console.WriteLine($"Request failed: {response.Status}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }
    }
}
