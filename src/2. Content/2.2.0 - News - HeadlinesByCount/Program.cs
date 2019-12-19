using Refinitiv.DataPlatform.Content.Data;
using Refinitiv.DataPlatform.Core;
using System;
using System.Linq;

// **********************************************************************************************************************
// 2.2.0 - News - HeadlinesByCount
// The following example demonstrates basic usage of the News interface to retrieve headlines.  Users can specify a
// specific query which will filter out news headlines based on criteria.  Optionally, they can specify the total maximum
// number of headlines to be returned.
//
// Note: To configure settings for your environment, visit the following files:
//      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
//      2. Configuration.Credentials to define your login credentials for the specified access channel.
// **********************************************************************************************************************
namespace NewsHeadlinesByCount
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

                // Default Count: Retrieve the most recent 100 headlines
                DisplayHeadlines(News.GetHeadlines());

                // Default Count: Retrieve most recent 100 headlines for Apple
                DisplayHeadlines(News.GetHeadlines(new Headlines.Params().WithQuery("R:AAPL.O")));

                // Specify Count: Retrieve most recent N headlines for Apple
                DisplayHeadlines(News.GetHeadlines(new Headlines.Params().WithQuery("R:AAPL.O")
                                                                         .WithCount(15)));

                // Specify Count: Retrieve large batch for the most recent N headlines for Apple
                DisplayHeadlines(News.GetHeadlines(new Headlines.Params().WithQuery("R:AAPL.O")
                                                                         .WithCount(350)));

                // Same as last one except provide a callback to retrieve each page
                DisplayHeadlines(News.GetHeadlines(new Headlines.Params().WithQuery("R:AAPL.O")
                                                                         .WithCount(350)
                                                                         .OnPageResponse((p, response) => DisplayHeadlines(response))));
            }
        }

        static void DisplayHeadlines(IHeadlinesResponse headlines)
        {
            if (headlines.IsSuccess)
            {
                Console.WriteLine($"\nRetrieved a total of {headlines.Data.Headlines.Count} headlines.  Small sample:");
                foreach (var headline in headlines.Data.Headlines.Take(2))
                    Console.WriteLine($"\t{headline.CreationDate}\t{headline.HeadlineTitle}");
            }
            else
                Console.WriteLine($"Issue retrieving headlines: {headlines.Status}");
        }
    }
}
