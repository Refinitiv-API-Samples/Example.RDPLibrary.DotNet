using Refinitiv.DataPlatform.Content.Pricing;
using Refinitiv.DataPlatform.Core;
using System;
using System.Linq;

namespace _2._2._05_Pricing_SnapshotChain
{
    // **********************************************************************************************************************
    // 2.2.05-Pricing-SnapshotChain
    // The following example demonstrates how to request and process a chain.  The interface supports the request/reply RDP
    // chain snapshot service.
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
                // Create a session into the platform
                using (ISession session = Configuration.Sessions.GetSession())
                {
                    if (session.Open() == Session.State.Opened)
                    {
                        IChainResponse response = Chains.Definition("0#.FCHI").GetData();

                        if (response.IsSuccess)
                        {
                            Console.WriteLine($"\nRetrieved Chain RIC: {response.Data.DisplayName}");

                            // Display the 30 first elements of the chain
                            int idx = 0;
                            foreach (string constituent in response.Data.Constituents.Take(30))
                            {
                                Console.WriteLine($"\t{++idx,2}. {constituent}");
                            }

                            if (response.Data.Constituents.Count > 30)
                            {
                                Console.WriteLine($"\t...\n\t<total of {response.Data.Constituents.Count} elements.>");
                            }
                        }
                        else
                        {
                            Console.WriteLine(response.Status);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }
    }
}
