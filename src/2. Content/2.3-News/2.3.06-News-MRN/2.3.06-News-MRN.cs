using Refinitiv.DataPlatform.Content.News;
using Refinitiv.DataPlatform.Core;
using System;
using System.Linq;

namespace _2._3._06_News_MRN
{
    // **********************************************************************************************************************
    // 2.3.06-News-MRN
    // The following example demonstrates how to subscribe to the Machine Readable News (MRN) service to retrieve any of the
    // available datafeeds (Headlines/Stories, TRNA or TRSI) within the MRN service.
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
                // Create a session into the platform...
                using (ISession session = Configuration.Sessions.GetSession())
                {
                    // Open the session
                    session.Open();

                    // Choose the type of data feed from the MRN service
                    string input;
                    do
                    {
                        Console.Write("\nChoose an MRN Data Feed (0 - Story, 1 - TRNA, 2- TRSI) (Enter to Exit): ");
                        input = Console.ReadLine();

                        if (input.Length > 0)
                        {
                            try
                            {
                                // Validate the selection
                                var feed = (MachineReadableNews.Datafeed)Enum.Parse(typeof(MachineReadableNews.Datafeed), input);

                                if (Enum.IsDefined(typeof(MachineReadableNews.Datafeed), feed))
                                {
                                    // Create a Machine Readable News (MRN) Item request
                                    using (var mrn = MachineReadableNews.Definition().OnError((stream, err) => Console.WriteLine($"{DateTime.Now}:{err}"))
                                                                                     .OnStatus((stream, status) => Console.WriteLine(status))
                                                                                     .NewsDatafeed(feed)
                                                                                     .OnNewsStory((stream, newsItem) => OnNewsStory(newsItem))
                                                                                     .OnNewsTrna((stream, newsItem) => OnNewsTrna(newsItem))
                                                                                     .OnNewsTrsi((stream, newsItem) => OnNewsTrsi(newsItem)))
                                    {
                                        mrn.Open();
                                        Console.ReadKey();
                                    }
                                }
                            }
                            catch (ArgumentException) { }
                        }
                    } while (input.Length > 0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }

        private static void OnNewsStory(IMRNStoryData news)
        {
            Console.WriteLine($"{ DateTime.Now:HH:mm:ss}. Story Body {news.NewsStory.Length} bytes ({news.HeadlineTitle})");
        }

        private static void OnNewsTrna(IMRNTrnaData news)
        {
            Console.WriteLine($"{ DateTime.Now:HH:mm:ss}. Scores for {news.Scores.Count} asset(s). ({news.HeadlineTitle})");
        }

        private static void OnNewsTrsi(IMRNTrsiData news)
        {
            Console.WriteLine($"{ DateTime.Now:HH:mm:ss}. Mov Averages for {news.Scores.Count} asset(s) => {string.Join(",", news.Scores.Select(w => w.AssetName))}");
        }
    }
}
