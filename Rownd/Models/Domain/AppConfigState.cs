using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rownd.Maui.Models.Domain
{
    public class AppState
    {
        [JsonIgnore]
        public bool IsLoading { get; set; } = false;
        public string Id { get; set; }
        public IList<string> UserVerificationFields { get; set; } = new List<string>();
        public AppConfig Config { get; set; } = new AppConfig();
    }

    public class AppConfig
    {
        public IList<Automation> Automations { get; set; } = new List<Automation>();
        public HubConfig Hub = new();
        public CustomizationsConfig Customizations = new();
        public string? Subdomain = null;
    }

    public class CustomizationsConfig
    {
        public string? PrimaryColor = null;
    }

    public class HubConfig
    {
        public HubCustomizationsConfig? Customizations = new();
    }

    public class HubCustomizationsConfig
    {
        public string? FontFamily { get; set; } = null;
        public string? DarkMode { get; set; } = null;
}

    public class Customizations
    {
        public string PrimaryColor { get; set; }
    }

    public class Automation
    {
        public string Id { get; set; }
        public string Template { get; set; }
        public string State { get; set; }

        public IList<AutomationRuleBase> Rules { get; set; }

        public IList<AutomationTrigger> Triggers { get; set; }
    }

    [JsonConverter(typeof(AutomationRuleConverter))]
    public class AutomationRuleBase { }

    public class AutomationRule : AutomationRuleBase
    {
        public string EntityType { get; set; }
        public string Attribute { get; set; }
        public string Condition { get; set; }
        public dynamic Value { get; set; }
    }

    public class AutomationOrRule : AutomationRuleBase
    {
        [JsonProperty("$or")]
        public IList<AutomationRule> Or { get; set; } = new List<AutomationRule>();
    }

    public class AutomationTrigger
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Target { get; set; }
    }

    public class AutomationRuleConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(AutomationRule).IsAssignableFrom(objectType) || typeof(AutomationOrRule).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            var isOrRule = jObject["$or"] != null;

            AutomationRuleBase target;
            if (isOrRule)
            {
                target = new AutomationOrRule();
            }
            else
            {
                target = new AutomationRule();
            }

            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            JObject jsonObject = new JObject();
            Type objectType = value.GetType();

            if (objectType == typeof(AutomationRule))
            {
                // Serialize AutomationRule properties dynamically
                SerializeProperties(jsonObject, (AutomationRule)value, serializer);
            }
            else if (objectType == typeof(AutomationOrRule))
            {
                // Serialize AutomationOrRule properties dynamically
                SerializeOrRuleProperties(jsonObject, (AutomationOrRule)value, serializer);
            }
            else
            {
                throw new ArgumentException("Object is not a valid AutomationRuleBase type.");
            }

            jsonObject.WriteTo(writer);
        }

        private void SerializeProperties(JObject jsonObject, AutomationRuleBase value, JsonSerializer serializer)
        {
            PropertyInfo[] properties = value.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.CanRead)
                {
                    object propertyValue = property.GetValue(value);
                    if (propertyValue == null)
                    {
                        continue;
                    }

                    jsonObject.Add(property.Name, JToken.FromObject(propertyValue, serializer));
                }
            }
        }

        private void SerializeOrRuleProperties(JObject jsonObject, AutomationRuleBase value, JsonSerializer serializer)
        {
            if (value is AutomationOrRule orRule)
            {
                jsonObject.Add("$or", JArray.FromObject(orRule.Or, serializer));
            }
        }
    }
}
