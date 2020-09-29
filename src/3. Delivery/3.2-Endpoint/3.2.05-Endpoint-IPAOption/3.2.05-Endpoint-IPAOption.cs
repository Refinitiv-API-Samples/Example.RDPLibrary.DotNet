using Newtonsoft.Json.Linq;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery.Request;
using System;

namespace _3._2._05_Endpoint_IPAOption
{
    class Program
    {
        static void Main(string[] _)
        {
            const string intradayEndpoint = "https://api.refinitiv.com/data/quantitative-analytics/v1/financial-contracts";

            try
            {
                // Create the platform session.
                using (ISession session = Configuration.Sessions.GetSession())
                {
                    // Open the session
                    session.Open();

                    // IPA - Financial Contracts (Option)
                    var response = Endpoint.SendRequest(session, intradayEndpoint,
                                                       new Endpoint.Request.Params().WithMethod(Endpoint.Request.Method.POST)
                                                                                    .WithBodyParameters(new JObject()
                                                                                    {
                                                                                        ["fields"] = new JArray("ErrorMessage",
                                                                                                                "UnderlyingRIC",
                                                                                                                "UnderlyingPrice",
                                                                                                                "DeltaPercent",
                                                                                                                "GammaPercent",
                                                                                                                "RhoPercent",
                                                                                                                "ThetaPercent",
                                                                                                                "VegaPercent"),
                                                                                        ["universe"] = new JArray(
                                                                                                        new JObject()
                                                                                                        {
                                                                                                            ["instrumentType"] = "Option",
                                                                                                            ["instrumentDefinition"] = new JObject()
                                                                                                            {
                                                                                                                ["instrumentCode"] = "FCHI560000L1.p",
                                                                                                                ["underlyingType"] = "Eti"
                                                                                                            },
                                                                                                            ["pricingParameters"] = new JObject()
                                                                                                            {
                                                                                                                ["underlyingTimeStamp"] = "Close"
                                                                                                            }
                                                                                                        })
                                                                                    }));
                    if (response.IsSuccess)
                    {
                        Console.WriteLine(response.Data.Raw["data"]);
                    }
                    else
                    {
                        Console.WriteLine($"Request failed: {response.Status}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\n**************\nFailed to execute: {e.Message}\n{e.InnerException}\n***************");
            }
        }
    }
}
