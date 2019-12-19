using Common_Examples;
using Refinitiv.DataPlatform.Content.Data;
using Refinitiv.DataPlatform.Core;
using System;

// **********************************************************************************************************************
// 2.3.1 - ESG - Measures
// The following example retrieves the environment measures for the specified list of companies.
//
// Note: To configure settings for your environment, visit the following files:
//      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
//      2. Configuration.Credentials to define your login credentials for the specified access channel.
// **********************************************************************************************************************
namespace ESG___Measures
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a session into the platform...
            ISession session = Configuration.Sessions.GetSession();

            // Open the session
            if ( session.Open() == Session.State.Opened )
            {

                // Show ESG measure scores with 2-year history
                Console.WriteLine("\nESG Measures Full based on company RICs...");
                Common.DisplayTable(EnvironmentalSocialGoverance.GetMeasuresFull(new EnvironmentalSocialGoveranceMeasuresFull.Params().Universe("IBM.N", "MSFT.O")
                                                                                                                                      .WithStart(-1)
                                                                                                                                      .WithEnd(0)),
                                    "ESG Measures Full");

                // Show ESG measure scores with 1-year history, based on a Perm ID
                Console.WriteLine("\nESG Measures Standard based on company Perm IDs...");
                Common.DisplayTable(EnvironmentalSocialGoverance.GetMeasuresStandard(new EnvironmentalSocialGoveranceMeasuresStandard.Params().Universe("4295904307", "8589934326")
                                                                                                                                              .WithStart(0)
                                                                                                                                              .WithEnd(0)),
                                    "ESG Measures Standard");
            }
        }
    }
}
