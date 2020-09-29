using System;
using Refinitiv.DataPlatform.Core;
using Refinitiv.DataPlatform.Delivery;

namespace Configuration
{
    public static class Sessions
    {
        public enum SessionTypeEnum
        {
            DESKTOP,                // DesktopSession           - Eikon/Refintiv Workspace
            RDP,                    // PlatformSession          - Refinitiv Data Platform
            DEPLOYED,               // PlatformSession          - Deployed ADS streaming services
            DEPLOYED_DEPRECATED     // DeployedPlatformSession  - (Deprecated) Deployed ADS Platform only
        };

        // Change the type of Session to switch the access channel
        public static SessionTypeEnum SessionType { get; set; } = SessionTypeEnum.RDP;

        
        
        // ** CHANGES BELOW ARE NOT REQUIRED UNLESS YOUR ACCESS REQUIRES ADDITIONAL PARAMETERS **

        // GetSession
        // Based on the above Session Type, retrieve the Session used to define how you want to access the platform.
        //
        public static ISession GetSession(bool overrideWebSocketIfNecessary=true)
        {
            if (overrideWebSocketIfNecessary)
            {
                // Note:
                // The default RDP Library for .NET WebSocket implementation is based on Microsoft's WebSocketClient.  This implementation
                // is only available on Windows 8 and above or if an application targets .NET Core 2.1 or greater.  Because all example
                // applications within this package are built using .NET Framework 4.5.2, if the Windows OS is anything less than Windows 8,
                // the WebSocket4Net implementation will be used.
                var ver = Environment.OSVersion.Version;
                if (ver.Major <= 6 && ver.Minor <= 1)
                {
                    DeliveryFactory.RegisterWebSocket(DeliveryFactory.WebSocketImpl.WebSocket4Net);
                }
            }
            
            switch (SessionType)
            {
                case SessionTypeEnum.RDP:
                    return (CoreFactory.CreateSession(new PlatformSession.Params()
                                                                .WithOAuthGrantType(new GrantPassword().UserName(Credentials.RDPUser)
                                                                                                       .Password(Credentials.RDPPassword))
                                                                .AppKey(Credentials.AppKey)
                                                                .WithTakeSignonControl(true)
                                                                .OnState((s, state, msg) => Console.WriteLine($"{DateTime.Now}:{msg}. (State: {state})"))
                                                                .OnEvent((s, eventCode, msg) => Console.WriteLine($"{DateTime.Now}:{msg}. (Event: {eventCode})"))));
                case SessionTypeEnum.DEPLOYED:
                    return CoreFactory.CreateSession(new PlatformSession.Params().WithHost(Credentials.TREPHost)
                                                                                 .OnState((s, state, msg) => Console.WriteLine($"{DateTime.Now}: State: {state}. {msg}"))
                                                                                 .OnEvent((s, eventCode, msg) => Console.WriteLine($"{DateTime.Now}: Event: {eventCode}. {msg}")));
                case SessionTypeEnum.DESKTOP:
                    return (CoreFactory.CreateSession(new DesktopSession.Params().AppKey(Credentials.AppKey)
                                                                                 .OnState((s, state, msg) => Console.WriteLine($"{DateTime.Now}:{msg}. (State: {state})"))
                                                                                 .OnEvent((s, eventCode, msg) => Console.WriteLine($"{DateTime.Now}:{msg}. (Event: {eventCode})"))));
                case SessionTypeEnum.DEPLOYED_DEPRECATED:
                    return (CoreFactory.CreateSession(new DeployedPlatformSession.Params().Host(Credentials.TREPHost)
                                                                                          .OnState((s, state, msg) => Console.WriteLine($"{DateTime.Now}:{msg}. (State: {state})"))
                                                                                          .OnEvent((s, eventCode, msg) => Console.WriteLine($"{DateTime.Now}:{msg}. (Event: {eventCode})"))));
                default:
                    throw new IndexOutOfRangeException($"Unknown Session Type: {SessionType}");
            }
        }
    }
}
