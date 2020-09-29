using Newtonsoft.Json.Linq;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery;
using Refinitiv.DataPlatform.Delivery.Queue;
using System;
using System.Collections.Generic;

namespace _3._3._03_Queue_Research
{
    // **********************************************************************************************************************
    // 3.3.03-Queue-Research
    // The Refinitiv Data Platform defines a service that utilizes Cloud-based Queuing to deliver realtime messages.
    // The following example demonstrates how to manage (retrieve/create/delete) a queue to deliver real-time research.
    // Prior to exit, the user is presented with the decision to delete the queue.
    //
    // Note: To configure settings for your environment, visit the following files within the .Solutions folder:
    //      1. Configuration.Session to specify the access channel into the platform. Default: RDP (PlatformSession).
    //      2. Configuration.Credentials to define your login credentials for the specified access channel.
    // **********************************************************************************************************************
    class Program
    {
        static void Main(string[] _)
        {
            const string researchEndpoint = "https://api.refinitiv.com/message-services/v1/research/subscriptions";

            try
            {
                using (ISession session = Configuration.Sessions.GetSession())
                {
                    if (session.Open() == Session.State.Opened)
                    {
                        // Create a QueueManager to actively manage our queues
                        IQueueManager manager = DeliveryFactory.CreateQueueManager(new QueueManager.Params().Session(session)
                                                                                                            .Endpoint(researchEndpoint)
                                                                                                            .OnError((qm, err) => Console.WriteLine(err)));

                        // First, check to see if we have any news headline queues active in the cloud...
                        List<IQueue> queues = manager.GetAllQueues();

                        // Check the error property to determine the result of the last request
                        if (manager.Error == null)
                        {

                            // Prepare Research criteria if we plan to create a new AWS queue - we must supply a research ID.                
                            JObject criteria = new JObject()
                            {
                                ["transport"] = new JObject()
                                {
                                    ["transportType"] = "AWS-SQS"
                                },
                                ["payloadVersion"] = "2.0",
                                ["userID"] = Configuration.Credentials.ResearchID
                            };

                            // If no existing queue exists, create one.
                            IQueue queue = (queues.Count > 0 ? queues[0] : manager.CreateQueue(criteria));

                            if (queue != null)
                            {
                                Console.WriteLine($"{Environment.NewLine}{(queues.Count > 0 ? "Using existing" : "Created a new")} queue...");

                                // Subscribe to the queue.
                                // Note: The subscriber interface has 2 mechanisms to retrieve data from the queue.  The first mechanism is to selectively
                                //       poll the queue for new messages.  The second mechanism is to define a callback/lambda expression and notify the
                                //       the subscriber to poll for messages as they come in - this mechansim provides a near realtime result.
                                //
                                // The following example demonstrates the first mechanism.
                                IQueueSubscriber subscriber = DeliveryFactory.CreateQueueSubscriber(new AWSQueueSubscriber.Params().Queue(queue));

                                Console.WriteLine("Attempt to retrieve research messages.  Hit any key to interrupt fetching...");

                                // Instead of defining a lambda callback, we manually poll the queue until we hit any key on the keyboard.
                                // Each poll will timeout after 5 seconds.
                                bool noMsgAvailable = false;
                                while (!Console.KeyAvailable)
                                {
                                    IQueueResponse result = subscriber.GetNextMessage(5);
                                    if (result.IsSuccess)
                                    {
                                        if (result.IsMessageAvailable)
                                            DisplayResearch(result);
                                        else
                                            Console.Write(noMsgAvailable ? "." : "No Message available from GetNextMessage");
                                        noMsgAvailable = !result.IsMessageAvailable;
                                    }
                                    else
                                    {
                                        Console.WriteLine(result.Status);
                                    }
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
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }

        private static void DisplayResearch(IQueueResponse response)
        {
            try
            {
                if (response.IsMessageAvailable)
                {
                    var msg = response.Data.Raw;

                    DateTime distributed = DateTime.Parse(msg["distributionTimestamp"].ToString()).ToLocalTime();

                    Console.WriteLine($"\nDocument distributed: {distributed}");

                    // Headline
                    var headline = msg["payload"]?["Headline"]?["DocumentHeadlineValue"]?.ToString();
                    if (headline != null)
                        Console.WriteLine($"\tHeadline:\t{headline}");

                    // Synopsis
                    var synopsis = msg["payload"]?["Synopsis"]?["DocumentSynopsisValue"]?.ToString();
                    if (synopsis != null)
                        Console.WriteLine($"\tSynopsis:\t{synopsis}");

                    // Document name
                    var document = msg["payload"]?["DocumentFileName"]?.ToString();
                    if (document != null)
                        Console.WriteLine($"\tDocument:\t{document} ({msg["payload"]?["DocumentFileType"]})");
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
}
