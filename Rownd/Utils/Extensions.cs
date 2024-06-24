using System;
namespace Rownd.Maui.Utils
{
    public static class Extensions
    {
        public static string ToUniversalISO8601(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("u").Replace(" ", "T");
        }

        public static DateTime? FromUniversalISO8601(string isoDateString)
        {
            try
            {
                return DateTime.Parse(isoDateString.Replace("T", " "));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
