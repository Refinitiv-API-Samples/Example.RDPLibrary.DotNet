using Refinitiv.DataPlatform.Content.Data;
using System;
using System.Data;

// **********************************************************************************************************************
// Common
// Convenient methods used across the projects defined within the Content layer.
// **********************************************************************************************************************
namespace Common_Examples
{
    public class Common
    {
        // **************************************************************************************************************************************
        // DisplayTable
        //
        // Convenience routine to layout columns and rows of data contained within the response.  Data is echoed to the console.
        // **************************************************************************************************************************************
        public static void DisplayTable(IDataSetResponse response, string header)
        {
            if (response.IsSuccess)
            {
                Console.WriteLine($"{Environment.NewLine}{header} for item(s): {String.Join(",", response.Data.Universe.ToArray())}");

                if (response.Data?.Table != null)
                {
                    Console.WriteLine();
                    foreach (DataColumn col in response.Data.Table.Columns)
                        Console.Write($"{col}\t");

                    Console.WriteLine();
                    foreach (DataRow dataRow in response.Data.Table.Rows)
                    {
                        foreach (var item in dataRow.ItemArray)
                            Console.Write($"{item}\t");
                        Console.WriteLine();
                    }
                }
                else
                    Console.WriteLine($"Response contains an empty data set: {response.Data.Raw}");
            }
            else
                Console.WriteLine(response.Status);

            Console.WriteLine();
        }
    }
}
