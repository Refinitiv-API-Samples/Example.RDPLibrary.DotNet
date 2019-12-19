using System;
using Refinitiv.DataPlatform.Core;

namespace Configuration
{
    public static class Sessions
    {
        public enum SessionTypeEnum
        {
            DESKTOP,        // DesktopSession           - Eikon/Refintiv Workspace
            RDP,            // PlatformSession          - Refinitiv Data Platform 
            DEPLOYED        // DeployedPlatformSession  - Deplayed TREP/ADS streaming services
        };

        // Change the type of Session to switch the access channel
        public static SessionTypeEnum SessionType { get; set; } = SessionTypeEnum.RDP;

        // ** CHANGES BELOW ARE NOT REQUIRED UNLESS YOUR ACCESS REQUIRES ADDITIONAL PARAMETERS **

        // GetSession
        // Based on the above Session Type, retrieve the Session used to define how you want to access the platform.
        //
        public static ISession GetSession()
        {
            switch (SessionType)
            {
                case SessionTypeEnum.RDP:
                    return (CoreFactory.CreateSession(new PlatformSession.Params()
                                                                .OAuthGrantType(new GrantPassword().UserName(Credentials.RDPUser)
                                                                                                   .Password(Credentials.RDPPassword))
                                                                .AppKey(Credentials.AppKey)
                                                                .WithTakeSignonControl(true)
                                                                .OnState((s, state, msg) => Console.WriteLine($"{DateTime.Now}:{msg}. (State: {state})"))
                                                                .OnEvent((s, eventCode, msg) => Console.WriteLine($"{DateTime.Now}:{msg}. (Event: {eventCode})"))));
                case SessionTypeEnum.DEPLOYED:
                    return (CoreFactory.CreateSession(new DeployedPlatformSession.Params().Host(Credentials.TREPHost)
                                                                                          .OnState((s, state, msg) => Console.WriteLine($"{DateTime.Now}:{msg}. (State: {state})"))
                                                                                          .OnEvent((s, eventCode, msg) => Console.WriteLine($"{DateTime.Now}:{msg}. (Event: {eventCode})"))));
                case SessionTypeEnum.DESKTOP:
                    return (CoreFactory.CreateSession(new DesktopSession.Params().AppKey(Credentials.AppKey)
                                                                                 .OnState((s, state, msg) => Console.WriteLine($"{DateTime.Now}:{msg}. (State: {state})"))
                                                                                 .OnEvent((s, eventCode, msg) => Console.WriteLine($"{DateTime.Now}:{msg}. (Event: {eventCode})"))));
                default:
                    throw new IndexOutOfRangeException($"Unknown Session Type: {SessionType}");
            }
        }
    }
}
