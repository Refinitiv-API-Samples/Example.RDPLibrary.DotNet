using Newtonsoft.Json.Linq;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery;
using Refinitiv.DataPlatform.Delivery.Request;
using System;

namespace _3._2._04_Endpoint_DataGrid
{
    class Program
    {
        static void Main(string[] args)
        {
            const string dataGridEndpoint = "https://api.refinitiv.com/data/datagrid/beta1/";

            try
            {
                // Create the platform session.
                using (ISession session = Configuration.Sessions.GetSession())
                {
                    // Open the session
                    session.Open();

                    IEndpoint endpoint = DeliveryFactory.CreateEndpoint(new Endpoint.Params().Session(session)
                                                                                             .Url(dataGridEndpoint));

                    // Simple request
                    var response = endpoint.SendRequest(new Endpoint.Request.Params().WithMethod(Endpoint.Request.Method.POST)
                                                                                     .WithBodyParameters(new JObject()
                                                                                     {
                                                                                     {"universe", new JArray("TRI.N", "IBM.N") },
                                                                                     {"fields", new JArray("TR.Revenue", "TR.GrossProfit") }
                                                                                     }));
                    DisplayResponse(response);

                    // Global parameters
                    response = endpoint.SendRequest(new Endpoint.Request.Params().WithMethod(Endpoint.Request.Method.POST)
                                                                                 .WithBodyParameters(new JObject()
                                                                                 {
                                                                                {"universe", new JArray("GOOG.O", "AAPL.O") },
                                                                                {"fields", new JArray("TR.Revenue", "TR.GrossProfit") },
                                                                                {"parameters", new JObject()
                                                                                    {
                                                                                        {"SDate", "0CY" },
                                                                                        {"Curn", "CAD" }
                                                                                    }
                                                                                }
                                                                                 }));
                    DisplayResponse(response);

                    // Historical data with specific date range
                    response = endpoint.SendRequest(new Endpoint.Request.Params().WithMethod(Endpoint.Request.Method.POST)
                                                                                 .WithBodyParameters(new JObject()
                                                                                 {
                                                                                 { "universe", new JArray("BHP.AX") },
                                                                                 { "fields", new JArray("TR.AdjmtFactorAdjustmentDate", "TR.AdjmtFactorAdjustmentFactor") },
                                                                                 { "parameters", new JObject()
                                                                                    {
                                                                                     {"SDate", "1980-01-01" },
                                                                                     {"EDate", "2018-09-29" }
                                                                                    }
                                                                                 }
                                                                                 }));
                    DisplayResponse(response);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }

        static void DisplayResponse(IEndpointResponse response)
        {
            if (response.IsSuccess)
            {
                Console.WriteLine(response.Data.Raw["universe"]);
                Console.WriteLine(response.Data.Raw["data"]);
            }
            else
                Console.WriteLine(response.Status);

            Console.WriteLine("Enter to continue...");
            Console.ReadLine();
        }
    }
}
