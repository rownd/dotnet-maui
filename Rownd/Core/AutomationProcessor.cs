using Microsoft.Extensions.Logging;
using Rownd.Maui.Utils;

namespace Rownd.Maui.Core
{
    using Rownd.Maui.Models.Domain;

    public static class AutomationProcessor
    {
        public static void RunAutomationsIfNeeded(GlobalState state)
        {
            try
            {
                var shouldRunAutomations = DoAutomationsNeedToRun(state);
                Loggers.Default.LogDebug("Should automations run? {shouldRunAutomations}", shouldRunAutomations);

                if (shouldRunAutomations && !Shared.Rownd.IsHubOpen())
                {
                    Task.Run(() =>
                    {
                        Shared.Rownd.DisplayHub(Hub.HubPageSelector.None);
                    });
                }
            }
            catch (Exception ex)
            {
                Loggers.Default.LogWarning("Failed to run automations: {ex}", ex);
            }
        }

        public static bool DoAutomationsNeedToRun(GlobalState state)
        {
            var automationMeta = ComputeAutomationMeta(state);
            return state.AppConfig.Config.Automations.Any(
                automation => automation.Rules.All(
                    rule => ProcessRule(rule, automationMeta, state)
                )
            );
        }

        private static bool ProcessRule(AutomationRuleBase rule, Dictionary<string, dynamic> automationMeta, GlobalState state)
        {
            if (rule is AutomationOrRule aRule1)
            {
                return aRule1.Or.Any(rule => ProcessRule(rule, automationMeta, state));
            }

            var aRule = (AutomationRule)rule;

            var data = aRule.EntityType == "user_data" ? state.User.Data : automationMeta;
            data.TryGetValue(aRule.Attribute, out var value);

            switch (aRule.Condition)
            {
                case "EQUALS":
                    return data[aRule.Attribute] == aRule.Value;

                case "EXISTS":
                    return value is string ? !string.IsNullOrWhiteSpace(value) : value != null;

                case "NOT_EXISTS":
                    return value is string ? string.IsNullOrWhiteSpace(value) : value == null;

                case "DATE_IS_BEFORE":
                    try
                    {
                        DateTime conditionDate, dataDate;
                        DateTime.TryParse(aRule.Value, out conditionDate);
                        DateTime.TryParse(value, out dataDate);
                        return dataDate == null || dataDate < conditionDate;
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                default:
                    return false;
            }
        }

        private static Dictionary<string, dynamic> ComputeAutomationMeta(GlobalState state)
        {
            return new Dictionary<string, dynamic>
            {
                { "is_authenticated", !string.IsNullOrWhiteSpace(state.Auth?.AccessToken) && state.Auth?.IsAccessTokenValid == true },
            };
        }
    }
}
