using Newtonsoft.Json.Linq;
using Refinitiv.DataPlatform.Content.News;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery;
using Refinitiv.DataPlatform.Delivery.Queue;
using System;
using System.Collections.Generic;
using System.Text;

namespace _3._3._01_Queue_NewsHeadlines
{
    // **********************************************************************************************************************
    // 3.3.01-Queue-Headlines
    // The Refinitiv Data Platform defines a service that utilizes Cloud-based Queuing to deliver realtime messages.
    // The following example demonstrates how to manage (retrieve/create/delete) a queue to deliver news headlines.  When
    // creating the queue, the example will use a news query expression to filter on only headlines that are classified as
    // news alerts.  Prior to exit, the user is presented with the decision to delete the queue.
    //
    // Note: To configure settings for your environment, visit the following files within the .Solutions folder:
    //      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
    //      2. Configuration.Credentials to define your login credentials for the specified access channel.
    // **********************************************************************************************************************
    class Program
    {
        static void Main(string[] _)
        {
            const string newsHeadlinesEndpoint = "https://api.refinitiv.com/message-services/v1/news-headlines/subscriptions";

            try
            {
                // Create the platform session.
                using (ISession session = Configuration.Sessions.GetSession())
                {
                    // Open the session
                    session.Open();

                    // Create a QueueManager to actively manage our queues
                    IQueueManager manager = DeliveryFactory.CreateQueueManager(new QueueManager.Params().Session(session)
                                                                                                        .Endpoint(newsHeadlinesEndpoint)
                                                                                                        .OnError((qm, err) => Console.WriteLine(err)));

                    // First, check to see if we have any news headline queues active in the cloud...
                    List<IQueue> queues = manager.GetAllQueues();

                    // Check the error property to determine the result of the last request
                    if (manager.Error == null)
                    {
                        // Determine if we retrieved an active headline queue...create one if not.
                        IQueue queue;
                        if (queues.Count > 0)
                            queue = queues[0];
                        else
                            queue = CreateQueue(manager, "AA");      // Create a Queue with the new query expression ("AA" - alerts only)

                        // Ensure our queue is created
                        if (queue != null)
                        {
                            Console.WriteLine($"{Environment.NewLine}{(queues.Count > 0 ? "Using existing" : "Created a new")} queue.  Waiting for headlines...");

                            // Subscribe to the queue.
                            // Note: The subscriber interface has 2 mechanisms to retrieve data from the queue.  The first mechanism is to selectively
                            //       poll the queue for new messages.  The second mechanism is to define a callback/lambda expression and notify the
                            //       the subscriber to poll for messages as they come in - this mechansim provides a near realtime result.
                            //
                            // The following example demonstrates the first mechanism.
                            IQueueSubscriber subscriber = DeliveryFactory.CreateQueueSubscriber(new AWSQueueSubscriber.Params().Queue(queue));

                            // Poll the queue until we hit any key on the keyboard.
                            // Each poll will timeout after 5 seconds.
                            while (!Console.KeyAvailable)
                            {
                                IQueueResponse result = subscriber.GetNextMessage(5);
                                if (result.IsSuccess)
                                    DisplayHeadline(result);
                                else
                                    Console.WriteLine(result.Status);
                            }
                            Console.ReadKey();

                            // Prompt the user to delete the queue
                            Console.Write("\nDelete the queue (Y/N) [N]: ");
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
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }


        // CreateQueue
        // The IQueueManager interface can accept a news expression to filter the type of news based on a JSON interface that
        // contains complicated expressions.  To simplify the specification of an expresion, RDP offers a news Analyze endpoint
        // that will take a familar desktop news expression and turn it into a JSON representation required by the queue manager.
        private static IQueue CreateQueue(IQueueManager manager, string newsExpression)
        {
            // Analyze the following news query string
            var analyze = Analyze.Definition(newsExpression).GetData();
            if (analyze.IsSuccess)
            {
                return manager.CreateQueue(analyze.Data);
            }
            else
            {
                Console.WriteLine($"Issues analyzing newsExpression [{newsExpression}\n{analyze.Status}\nCreating queue with no criteria.");
                return manager.CreateQueue();
            }
        }

        // DisplayHeadline
        // The raw message from the platform is interrogated to pull out the headline.  The 'pubStatus' indicator determines
        // if the headline is 'usable'.  A non-usable headline (canceled or withdrawn) will not carry any headline title to display.
        private static void DisplayHeadline(IQueueResponse response)
        {
            try
            {
                if (response.IsMessageAvailable)
                {
                    var msg = response.Data.Raw;

                    // Determine if the headline is usable, i.e. if we want to display it
                    var pubStatus = msg["payload"]?["newsItem"]?["itemMeta"]?["pubStatus"]?["_qcode"]?.ToString();
                    if (pubStatus is string && pubStatus.Contains("usable"))
                    {
                        DateTime local = DateTime.Parse(msg["distributionTimestamp"].ToString()).ToLocalTime();

                        // Determine if this is an actual headline
                        JArray headline = msg["payload"]?["newsItem"]?["contentMeta"]?["headline"] as JArray;
                        if (headline?.Count > 0 && headline[0]["$"] is JToken)
                            Console.WriteLine($"{local}: {headline[0]["$"]}".Indent(110));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("*************************************************");
                Console.WriteLine(e);
                Console.WriteLine(response.Data.Raw);
                Console.WriteLine("*************************************************");
            }
        }
    }

    public static class StringExtensions
    {
        public static string Indent(this string value, int maxSize)
        {
            StringBuilder sb = new StringBuilder();
            string str = value;
            int indent = 0;
            while (str.Length > maxSize)
            {
                sb.Append(new string(' ', indent)).Append(str.Substring(0, maxSize));
                sb.Append(Environment.NewLine);
                indent = 23;
                str = str.Substring(maxSize);
            }

            sb.Append(new string(' ', indent)).Append(str);

            return sb.ToString();
        }
    }
}
