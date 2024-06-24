using System;
using Newtonsoft.Json;
using Rownd.Maui.Utils;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;

namespace Rownd.Maui.Core
{
    public class Customizations
    {
        [JsonConverter(typeof(JsonColorHexConverter))]
        public Color SheetBackgroundColor { get; set; }

        [JsonIgnore]
        public Color DynamicSheetBackgroundColor
        {
            get
            {
                AppTheme currentTheme = Application.Current.RequestedTheme;
                return SheetBackgroundColor != null
                    ? SheetBackgroundColor
                    : (currentTheme == AppTheme.Dark
                        ? Color.FromArgb("#1c1c1e")
                        : Color.FromArgb("#ffffff"));
            }
        }

        [JsonIgnore]
        public Color PrimaryForegroundColor { get; set; }

        public int SheetCornerBorderRadius { get; set; } = 12;

        public double DefaultFontSize { get; set; } = // TODO Xamarin.Forms.Device.GetNamedSize is not longer supported. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
Device.GetNamedSize(NamedSize.Medium, typeof(Label)) * 0.6;

        public Customizations()
        {
            AppTheme currentTheme = Application.Current.RequestedTheme;

            switch (currentTheme)
            {
                case AppTheme.Unspecified:
                case AppTheme.Light:
                    {
                        SheetBackgroundColor = Colors.White;
                        PrimaryForegroundColor = Color.FromArgb("#222222");
                        break;
                    }

                case AppTheme.Dark:
                    {
                        SheetBackgroundColor = Color.FromArgb("#333333");
                        PrimaryForegroundColor = Colors.White;
                        break;
                    }
            }
        }
    }
}