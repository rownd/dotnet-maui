using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Rownd.Maui.Utils;
using Microsoft.Maui.Devices;

namespace Rownd.Maui.Core
{
    public static class Constants
    {
        private static string sdkVersion = Assembly.GetAssembly(typeof(Constants)).GetName().Version.ToString();

        public static readonly string DEFAULT_API_USER_AGENT = $"Rownd SDK for Xamarin/{sdkVersion} (Language C#; Platform={DeviceInfo.Platform} {DeviceInfo.VersionString}; (SDK {sdkVersion});)";
        public static readonly string DEFAULT_WEB_USER_AGENT = $"Rownd SDK for Xamarin/{sdkVersion} (Language C#; Platform={DeviceInfo.Platform} {DeviceInfo.VersionString}; (SDK {sdkVersion});)";

        public static string GetSupportedFeatures()
        {
            var supportedFeatures = new List<SupportedFeature>()
            {
                new SupportedFeature
                {
                    Name = "openEmailInbox",
                    Enabled = "true"
                },
                new SupportedFeature
                {
                    Name = "can_receive_event_messages",
                    Enabled = "true"
                }
            };

            return JsonConvert.SerializeObject(supportedFeatures);
        }
    }
}
