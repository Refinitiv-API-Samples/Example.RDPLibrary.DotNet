using Refinitiv.DataPlatform.Content.Data;
using Refinitiv.DataPlatform.Core;
using System;

// **********************************************************************************************************************
// 2.2.2 - News - Story
// The following example presents a news story based on a specific headline.
//
// Note: To configure settings for your environment, visit the following files:
//      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
//      2. Configuration.Credentials to define your login credentials for the specified access channel.
// **********************************************************************************************************************
namespace NewsStory
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

                // Retrieve the most recent headline about Apple
                var headline = News.GetHeadlines(new Headlines.Params().WithQuery("L:EN and Apple")
                                                                       .WithCount(1));

                if (headline.IsSuccess)
                {
                    // Retrieve the story based on the story ID
                    var story = News.GetStory(headline.Data.Headlines[0].StoryId);

                    Console.WriteLine($"\nHeadline: {headline.Data.Headlines[0].HeadlineTitle}");

                    if (story.IsSuccess)
                        Console.WriteLine($"\nStory: {story.Data.NewsStory}");
                    else
                        Console.WriteLine($"Problem retrieving the story: {story.Status}");
                }
                else
                    Console.WriteLine($"Problem retrieving the headline: {headline.Status}");
            }
        }
    }
}
