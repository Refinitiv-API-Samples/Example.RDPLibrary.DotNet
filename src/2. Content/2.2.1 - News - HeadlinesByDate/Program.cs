using Refinitiv.DataPlatform.Content.Data;
using Refinitiv.DataPlatform.Core;
using System;
using System.Linq;

// **********************************************************************************************************************
// 2.2.1 - News - HeadlinesByDate
// The News interfaces provide options to query for headlines within a specified time period.  The following example 
// demonstrates the behavior and how to retrieve headlines based on a time period.
//
// Note: To configure settings for your environment, visit the following files:
//      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
//      2. Configuration.Credentials to define your login credentials for the specified access channel.
// **********************************************************************************************************************
namespace NewsHeadlinesByDate
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

                // Use date specified within query: "Apple daterange:'2019-06-01,2019-06-07'"
                Console.Write("\nRetrieve all headlines for query: 'Apple daterange'...");
                DisplayHeadlines(News.GetHeadlines(new Headlines.Params().WithQuery(@"Apple daterange:""2019-06-01,2019-06-07""")
                                                                         .WithCount(0)
                                                                         .WithSort(Headlines.SortOrder.oldToNew)));

                // Use date specifier within query - last 5 days
                Console.Write("Retrieve all headlines for query: 'Apple last 5 days'...");
                DisplayHeadlines(News.GetHeadlines(new Headlines.Params().WithQuery("Apple last 5 days")
                                                                         .WithCount(0)
                                                                         .WithSort(Headlines.SortOrder.oldToNew)));

                // Same as previous except show each page response from the platform
                Console.Write("Same as previous except show each page response...");
                DisplayHeadlines(News.GetHeadlines(new Headlines.Params().WithQuery("Apple last 5 days")
                                                                         .WithCount(0)
                                                                         .OnPageResponse((p, headlines) => Console.Write($"{headlines.Data.Headlines.Count}, "))
                                                                         .WithSort(Headlines.SortOrder.oldToNew)));
            }
        }

        static void DisplayHeadlines(IHeadlinesResponse headlines)
        {
            if (headlines.IsSuccess)
            {
                Console.WriteLine($"Retrieved a total of {headlines.Data.Headlines.Count} headlines.  Small sample:");
                foreach (var headline in headlines.Data.Headlines.Take(2))
                    Console.WriteLine($"\t{headline.CreationDate}\t{headline.HeadlineTitle}");
            }
            else
                Console.WriteLine($"Issue retrieving headlines: {headlines.Status}");

            Console.WriteLine("\n************************************************************************\n");
        }
    }
}
