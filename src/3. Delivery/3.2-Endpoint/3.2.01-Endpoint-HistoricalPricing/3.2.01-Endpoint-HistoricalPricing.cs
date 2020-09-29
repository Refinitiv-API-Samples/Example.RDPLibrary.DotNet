using Newtonsoft.Json.Linq;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery.Request;
using System;

namespace _3._2._01_Endpoint_HistoricalPricing
{
    // **********************************************************************************************************************
    // 3.2.01-Endpoint-HistoricalPricing
    // The following example demonstrates basic usage of the RDP Library for .NET to request data using the Delivery Endpoint 
    // interface. The example will refer to the Historical Pricing Events API to retrieve time series pricing events such as 
    // trades, quotes and corrections.
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
                    // Basic Endpoint retrieval.
                    // The endpoint URL contains all required parameters.
                    // ********************************************************************************************
                    var endpointUrl = "https://api.refinitiv.com/data/historical-pricing/v1/views/events/VOD.L";

                    // Request and display the timeseries data.
                    DisplayResult(Endpoint.SendRequest(session, endpointUrl));

                    // Specify a query parameter to control the number of elements returned for this endpoint.
                    DisplayResult(Endpoint.SendRequest(session, endpointUrl, new Endpoint.Request.Params().WithQueryParameter("count", "5")));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }

        // DisplayResult
        // The following code segment interrogates the response returned from the platform.  If the request was successful, simple details
        // related to the pricing data is displayed.  Otherwise, status details outlining failure reasons are presented. 
        static void DisplayResult(IEndpointResponse response)
        {
            if (response.IsSuccess)
            {
                // Retrieve the data elements from the response and display how many rows of historical pricing content came back.
                var data = response.Data?.Raw[0]?["data"] as JArray;
                Console.WriteLine($"Timeseries data response: {response.Data?.Raw}{Environment.NewLine}A total of {data?.Count} elements returned.");
            }
            else
                Console.WriteLine($"Failed to retrieve data: {response.Status}");

            Console.Write("Hit enter to continue..."); Console.ReadLine();
        }
    }
}
