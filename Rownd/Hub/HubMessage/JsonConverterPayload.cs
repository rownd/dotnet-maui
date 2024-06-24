using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rownd.Maui.Hub.HubMessage
{
    public class JsonConverterPayload : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Message);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            Message message = new Message();
            var typeString = (string)jo["type"];

            Type enumType = objectType.GetProperty("Type").PropertyType;
            var enumMemberValues = GetEnumMemberValues(enumType);

            string enumMemberName = enumMemberValues.FirstOrDefault(pair => pair.Value == typeString).Key;

            if (enumMemberName == null)
            {
                throw new NotSupportedException($"Payload type '{typeString}' is not supported.");
            }

            if (!Enum.TryParse(enumMemberName, out MessageType type))
            {
                throw new NotSupportedException($"Payload type '{typeString}' is not supported.");
            }

            message.Type = type;

            PayloadBase payload = null;

            // TODO: There's probably a more efficient way to do this
            switch (type)
            {
                case MessageType.Authentication:
                    {
                        payload = jo["payload"].ToObject<PayloadAuthenticated>(serializer);
                        break;
                    }

                case MessageType.HubResize:
                    {
                        payload = jo["payload"].ToObject<PayloadHubResize>(serializer);
                        break;
                    }

                case MessageType.CanTouchBackgroundToDismiss:
                    {
                        payload = jo["payload"].ToObject<PayloadCanTouchBackgroundToDismiss>(serializer);
                        break;
                    }

                case MessageType.UserDataUpdate:
                    {
                        payload = jo["payload"].ToObject<PayloadUserDataUpdate>(serializer);
                        break;
                    }

                case MessageType.Event:
                    {
                        payload = jo["payload"].ToObject<PayloadEvent>(serializer);
                        break;
                    }

                case MessageType.AuthChallengeInitiated:
                    {
                        payload = jo["payload"].ToObject<PayloadAuthChallengeInitiated>(serializer);
                        break;
                    }
            }

            message.Payload = payload;

            return message;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private string GetEnumMemberValue(string enumString)
        {
            var enumType = typeof(MessageType);
            var memberInfos = enumType.GetMembers(BindingFlags.Public | BindingFlags.Static);

            foreach (var memberInfo in memberInfos)
            {
                var attribute = memberInfo.GetCustomAttribute<EnumMemberAttribute>();
                if (attribute != null && attribute.Value == enumString)
                {
                    return memberInfo.Name;
                }
            }

            return enumString;
        }

        private Dictionary<string, string> GetEnumMemberValues(Type enumType)
        {
            var enumMemberValues = new Dictionary<string, string>();

            foreach (var field in enumType.GetFields())
            {
                var attribute = field.GetCustomAttributes(typeof(EnumMemberAttribute), true)
                    .FirstOrDefault() as EnumMemberAttribute;

                if (attribute != null)
                {
                    enumMemberValues.Add(field.Name, attribute.Value);
                }
            }

            return enumMemberValues;
        }
    }
}
