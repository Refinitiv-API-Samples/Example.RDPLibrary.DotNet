using Common_Examples;
using Refinitiv.DataPlatform.Content.Data;
using Refinitiv.DataPlatform.Core;
using System;

// **********************************************************************************************************************
// 2.0.0 - HistoricalPricing - Summaries
// The HistoricalPricing Summaries example demonstrates both intraday and interday data retrievals from the platform.
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

                // Retrieve Intraday Summaries with PT1M (1-minute interval).  Specify to capture only 2 rows.
                var response = HistoricalPricing.GetSummaries(new HistoricalPricingSummaries.Params().Universe("VOD.L")
                                                                                                     .WithInterval(HistoricalPricingSummaries.Interval.PT1M)
                                                                                                     .WithCount(2));
                
                
                Common.DisplayTable(response, "Historical Intraday Summaries");

                // Retrieve Interday Summaries with P1D (1-day interval).  Specify to capture only 2 rows.
                response = HistoricalPricing.GetSummaries(new HistoricalPricingSummaries.Params().Universe("VOD.L")
                                                                                                 .WithInterval(HistoricalPricingSummaries.Interval.P1D)
                                                                                                 .WithCount(2));
                Common.DisplayTable(response, "Historical Interday Summaries");
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }
    }
}
