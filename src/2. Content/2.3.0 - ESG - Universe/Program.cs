using Common_Examples;
using Refinitiv.DataPlatform.Content.Data;
using Refinitiv.DataPlatform.Core;

// **********************************************************************************************************************
// 2.3.0 - ESG - Universe
// The following example retrieves the list of all organziations that have Environmental coverage.
//
// Note: To configure settings for your environment, visit the following files:
//      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
//      2. Configuration.Credentials to define your login credentials for the specified access channel.
// **********************************************************************************************************************
namespace ESG___Universe
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

                // List all organizations that have ESG coverage
                Common.DisplayTable(EnvironmentalSocialGoverance.GetUniverse(), "ESG Universe");
            }
        }
    }
}
