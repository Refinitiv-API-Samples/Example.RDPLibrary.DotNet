using Newtonsoft.Json.Linq;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery.Request;
using System;

namespace _3._2._02_Endpoing_News_Headline_and_Story
{
    // **********************************************************************************************************************
    // 3.2.02-Endpoint-News Headline and Story
    // The following example demonstrates the news headline and story API endpoints.  The code segment retrieves the first
    // headline and story text from the response.
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
                // Create the platform session.
                using (ISession session = Configuration.Sessions.GetSession())
                {
                    // Open the session
                    session.Open();

                    // ********************************************************************************************
                    // Define the news headline URL - we need this to retrieve the story ID.
                    // ********************************************************************************************
                    var headlineUrl = "https://api.refinitiv.com/data/news/v1/headlines";

                    // Request for the headline based on the following query
                    var query = "Apple searchIn:HeadlineOnly";
                    var response = Endpoint.SendRequest(session, headlineUrl, new Endpoint.Request.Params().WithQueryParameter("query", query));

                    // The headline response will carry the story ID.
                    var storyId = GetStoryId(response);
                    if (storyId != null)
                    {
                        Console.WriteLine($"\nRequesting story based on ID: {storyId}");

                        // Display the headline and story.  First, define the story endpoint Url.
                        // The URL contains a path token {storyId} which will be replaced by the story ID extracted.
                        var storyUrl = "https://api.refinitiv.com/data/news/v1/stories/{storyId}";

                        // Retrieve and display the story based on the ID we retrieved from the headline
                        DisplayHeadlineAndStory(Endpoint.SendRequest(session, storyUrl, new Endpoint.Request.Params().WithPathParameter("storyId", storyId)));
                    }
                    else
                        Console.WriteLine($"Problems retrieving the story ID:{Environment.NewLine}{response.Status}");

                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }

        // Retrieve the story ID from the first headline
        private static string GetStoryId(IEndpointResponse response)
        {
            string storyId = null;
            if (response.IsSuccess)
            {
                var data = response.Data?.Raw["data"] as JArray;

                // Retrieve the first headline, if available
                storyId = data?[0]?["storyId"]?.ToString();
            }

            return storyId;
        }

        // DisplayHeadlineAndStory
        // Interrogate the response to pull out the headline and story text.
        private static void DisplayHeadlineAndStory(IEndpointResponse response)
        {
            if (response.IsSuccess)
            {
                Console.Write($"{Environment.NewLine}Headline: ");
                Console.WriteLine(response.Data?.Raw["newsItem"]?["itemMeta"]?["title"]?[0]?["$"]);

                Console.Write($"{Environment.NewLine}Story: ");
                Console.WriteLine(response.Data?.Raw["newsItem"]?["contentSet"]?["inlineData"]?[0]?["$"]);
            }
            else
                Console.WriteLine($"Failed to retrieve data: {response.Status}");
        }
    }
}
