using Refinitiv.DataPlatform;
using Refinitiv.DataPlatform.Core;
using System;

namespace LogAPI
{
    // *******************************************************************************************************************************************
    // When using the Refinitiv Data Platform Library for .NET, general information log messages will be sent to a unique log file, by default.  
    // However, application developers can choose to override this behavior.
    // 
    // The following tutorial demonstrates the ability for an application to programmatically manage logging output.  Application developers can 
    // control the level of details (LogLevel) programmatically at runtime.  In addition, applications can choose to  programmatically capture all 
    // Refinitiv Data Platform Library log details to be used by their own logging frameworks.
    //
    // To demonstrate basic functionality, the application simply opens a session and exits. 
    // *******************************************************************************************************************************************
    class Program
    {
        static void Main(string[] args)
        {          
            // Override the default log level defined for the Refinitiv Library.
            Log.Level = NLog.LogLevel.Debug;

            // Intercept all Refinitiv Data Platform Library log messages within a lambda expression. In our case, the lambda expression 
            // simply echos all log messages generated within the library to the console.
            Log.Output = (loginfo, parms) => Console.WriteLine($"Application: {loginfo.Level} - {loginfo.FormattedMessage}");

            // Create the platform session.
            using (ISession session = Configuration.Sessions.GetSession())
            {
                // Open the session
                session.Open();
            }
        }
    }
}
