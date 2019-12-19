using Newtonsoft.Json.Linq;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery;
using Refinitiv.DataPlatform.Delivery.Queue;
using System;
using System.Collections.Generic;

// **********************************************************************************************************************
// 3.2.0 - Alerts - Headlines
// The Refinitiv Data Platform defines an Alert service that utilizes Cloud-based Queuing to deliver realtime messages.
// The following example demonstrates this cloud queuing mechanism to deliver realtime news headline alerts.  The example
// will decide to use a previously created queue or create one if none exist.  Once a queue has been acquired, the
// application will begin polling for headlines and display to the console.  When polling stops, the user is presented
// with the decision to delete the queue.
//
// Note: To configure settings for your environment, visit the following files:
//      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
//      2. Configuration.Credentials to define your login credentials for the specified access channel.
// **********************************************************************************************************************
namespace Queue
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

                // ********************************************************************************************
                // Headline Alert Endpoint URL.
                // ********************************************************************************************
                const string alertHeadlinesEndpoint = "https://api.refinitiv.com/alerts/v1/news-headlines-subscriptions";

                // Determine if we are using an existing queue or creating one.  The QueueManager will assist us here.
                IQueueManager manager = DeliveryFactory.CreateQueueManager(new QueueManager.Params().Session(session)
                                                                                                    .Endpoint(alertHeadlinesEndpoint)
                                                                                                    .OnError((qm, err) => Console.WriteLine(err)));

                // First, check to see if we have any news headline queues active in the cloud, if not, create one.
                List<IQueue> queues = manager.GetAllQueues();

                // Determine if we retrieved anything...create one if not.
                IQueue queue = (queues?.Count > 0 ? queues[0] : manager.CreateQueue());

                // Ensure our queue is created
                if (queue != null)
                {
                    Console.WriteLine($"{Environment.NewLine}{(queues.Count > 0 ? "Using existing" : "Created a new")} queue...");

                    // Start polling for news headline messages from the queue
                    // A QueueSubscriber provides the mechanisms to poll the queue and capture each headline alert via lambda expressions
                    IQueueSubscriber subscriber = DeliveryFactory.CreateQueueSubscriber(
                                                        new AWSQueueSubscriber.Params().Queue(queue)
                                                                                       .WithMessagePollingInterval(1)
                                                                                       .OnResponse((s, response) =>
                                                                                       {
                                                                                           if (response.IsSuccess)
                                                                                               DisplayHeadline(response.Data.Raw);
                                                                                           else
                                                                                               Console.WriteLine(response.Status);
                                                                                       }));

                    // Open the subscriber to begin polling for messages
                    subscriber.StartPolling();
                    Console.WriteLine("Polling for messages from the queue...hit any key to stop polling");

                    // Hit any key to stop...
                    Console.ReadKey();

                    // Close the subscription - stops polling for messages
                    subscriber.StopPolling();
                    Console.WriteLine($"{Environment.NewLine}Stopped polling for messages from the queue.");

                    // Prompt the user to delete the queue
                    Console.Write("Delete the queue (Y/N) [N]: ");
                    var delete = Console.ReadLine();
                    if (delete?.ToUpper() == "Y")
                    {
                        if (manager.DeleteQueue(queue))
                            Console.WriteLine("Successfully deleted queue.");
                        else
                            Console.WriteLine($"Issues deleting queue {manager.Error}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }

        // DisplayHeadline
        // Interrogate the body of the news response and pull out the headline text.
        static void DisplayHeadline(JToken msg)
        {
            try
            {
                var local = DateTime.Parse(msg["contentPublishTimestamp"].ToString()).ToLocalTime();

                // Determine if this is an actual headline
                var newsItem = msg["payload"]?["newsMessage"]?["itemSet"]?["newsItem"] as JArray;
                if (newsItem?.Count > 0)
                {
                    var headline = newsItem[0]["contentMeta"]?["headline"] as JArray;
                    if (headline?.Count > 0)
                        Console.WriteLine($"{local}: {headline[0]["$"]}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("*************************************************");
                Console.WriteLine(e);
                Console.WriteLine(msg);
                Console.WriteLine("*************************************************");
            }
        }
    }
}
