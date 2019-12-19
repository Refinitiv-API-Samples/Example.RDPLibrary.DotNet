using Common_Examples;
using Refinitiv.DataPlatform.Content.Data;
using Refinitiv.DataPlatform.Core;
using System;

// **********************************************************************************************************************
// 2.0.1 - HistoricalPricing - Events
// The HistoricalPricing Events example demonstrates how to capture trade tick-based data.
// The example uses a common method to display the table of data returned.
//
// Note: To configure settings for your environment, visit the following files:
//      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
//      2. Configuration.Credentials to define your login credentials for the specified access channel.
// **********************************************************************************************************************
namespace HistoricalPricingExample
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Create the platform session.
                ISession session = Configuration.Sessions.GetSession();

                // Open the session
                session.Open();

                // Retrieve tick pricing events.  Default: 20 rows of data.  Specified trades only and specific columns of data.
                var response = HistoricalPricing.GetEvents(new HistoricalPricingEvents.Params().Universe("VOD.L")
                                                                                               .WithEventTypes(HistoricalPricingEvents.EventType.trade)
                                                                                               .WithFields("DATE_TIME", "EVENT_TYPE", "TRDPRC_1", "TRDVOL_1"));
                
                Common.DisplayTable(response, "Historical Trade events");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }
    }
}
