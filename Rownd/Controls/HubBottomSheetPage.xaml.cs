using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using Rownd.Maui.Controls;
using Rownd.Maui.Core;
using Rownd.Maui.Hub;

#if ANDROID
using Rownd.Maui.Android;
#endif

namespace Rownd.Controls
{
    public partial class HubBottomSheetPage : ContentPage
    {
        #region Properties

        private static readonly BindableProperty InitialPositionProperty = BindableProperty.Create(
            nameof(InitialPosition),
            typeof(int),
            typeof(HubBottomSheetPage),
            defaultValue: 250,
            defaultBindingMode: BindingMode.OneTime);

        public int InitialPosition
        {
            get
            {
                return (int)GetValue(InitialPositionProperty);
            }
            set
            {
                SetValue(InitialPositionProperty, value);
                OnPropertyChanged();
            }
        }

        private static readonly BindableProperty IsDismissableProperty = BindableProperty.Create(
            nameof(IsDismissable),
            typeof(bool),
            typeof(HubBottomSheetPage),
            defaultValue: true,
            defaultBindingMode: BindingMode.TwoWay
        );

        public bool IsDismissable
        {
            get
            {
                return (bool)GetValue(IsDismissableProperty);
            }
            set
            {
                SetValue(IsDismissableProperty, value);
                OnPropertyChanged();
            }
        }

        private static readonly BindableProperty IsLoadingProperty = BindableProperty.Create(
            nameof(IsLoading),
            typeof(bool),
            typeof(HubBottomSheetPage),
            defaultValue: true,
            defaultBindingMode: BindingMode.OneWay
        );

        public bool IsLoading
        {
            get
            {
                return (bool)GetValue(IsLoadingProperty);
            }
            set
            {
                SetValue(IsLoadingProperty, value);
                OnPropertyChanged();
            }
        }

        private static readonly BindableProperty SheetBackgroundColorProperty = BindableProperty.Create(
            nameof(SheetBackgroundColor),
            typeof(Color),
            typeof(HubBottomSheetPage),
            defaultValue: Colors.White,
            defaultBindingMode: BindingMode.OneWay
        );

        public Color SheetBackgroundColor
        {
            get
            {
                return (Color)GetValue(SheetBackgroundColorProperty);
            }
            set
            {
                SetValue(SheetBackgroundColorProperty, value);
                OnPropertyChanged();
            }
        }

        private static readonly BindableProperty PrimaryForegroundColorProperty = BindableProperty.Create(
            nameof(PrimaryForegroundColor),
            typeof(Color),
            typeof(HubBottomSheetPage),
            defaultValue: Color.FromArgb("#333333"),
            defaultBindingMode: BindingMode.OneWay
        );

        public Color PrimaryForegroundColor
        {
            get
            {
                return (Color)GetValue(PrimaryForegroundColorProperty);
            }
            set
            {
                SetValue(PrimaryForegroundColorProperty, value);
                OnPropertyChanged();
            }
        }
        #endregion

        private bool isPanning = false;

        public event EventHandler? OnDismiss;

        public HubBottomSheetPage()
        {
            InitializeComponent();
            BindingContext = this;
            Setup();
        }

        internal HubWebView GetHubWebView()
        {
            return Webview;
        }

        private void Setup()
        {
            // OSAppTheme currentTheme = Shared.App.RequestedTheme;

            if (Webview is IBottomSheetChild child)
            {
                child.SetBottomSheetParent(this);
            }

            SheetBackgroundColor = Shared.ServiceProvider.GetService<Config>().Customizations.SheetBackgroundColor;
            PrimaryForegroundColor = Shared.ServiceProvider.GetService<Config>().Customizations.PrimaryForegroundColor;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                this.UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            }

            BackgroundColor = new Color(0, 0, 0, 0.01F);

            _ = AnimateIn();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.ResetWindowSoftInputModeAdjust();
            OnDismiss?.Invoke(this, new EventArgs());
        }

        private readonly uint duration = 300;
        private double currentPosition = 0;
        private double detentPoint = 500;

        internal double LastRequestedPosition { get; private set; } = 250;

        public async void OnBottomSheetPan(object sender, PanUpdatedEventArgs e)
        {
            if (!IsDismissable)
            {
                await Shake();
                return;
            }

            try
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Running:
                        isPanning = true;
                        var translateY = Math.Max(Math.Min(0, currentPosition + e.TotalY), -Math.Abs((Height * .05) - Height));
                        await Sheet.TranslateTo(Sheet.X, translateY, 20, Easing.SpringOut);

                        break;

                    case GestureStatus.Completed:
                        currentPosition = Sheet.TranslationY;
                        isPanning = false;

                        if (!IsSwipeUp(e) && (Math.Abs(currentPosition) < InitialPosition || Math.Abs(currentPosition) < Math.Abs(detentPoint + 100)))
                        {
                            await Dismiss();
                        }
                        else
                        {
                            // Snap to top or last detent
                            var currentHeight = Math.Abs(currentPosition);
                            var maxHeight = GetMaxHeight();
                            var maxDiff = Math.Abs(currentHeight - maxHeight);
                            var detentDiff = Math.Abs(currentHeight - Math.Abs(detentPoint));

                            double targetHeight = Math.Abs(detentPoint);
                            if (maxDiff < detentDiff)
                            {
                                targetHeight = maxHeight;
                            }

                            _ = AnimateTo(-targetHeight);
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public bool IsSwipeUp(PanUpdatedEventArgs e)
        {
            if (e.TotalY < 0)
            {
                return true;
            }

            return false;
        }

        // open sheet to 95% of the view
        public async Task Expand()
        {
            await RequestHeight(GetProportionCoordinate(.95));
        }

        /**
         * <summary>
         * Request a height for the sheet in device-independent pixels.
         * </summary>
         * <param name="height">A positive number to which the sheet height should adjust.</param>
         * <returns>A Task indicating when the height request has been fulfilled.</returns>
         * */
        public async Task RequestHeight(double height)
        {
            if (isPanning)
            {
                return;
            }

            LastRequestedPosition = height;
            await AnimateTo(-height);
        }

        public async Task Dismiss()
        {
            if (!IsDismissable)
            {
                await Shake();
                return;
            }

            await AnimateOut();

            if (Microsoft.Maui.Controls.Application.Current?.MainPage != null)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PopModalAsync(false);
            }
        }

        private double GetProportionCoordinate(double proportion)
        {
            Thickness insets = default;

            // TODO Xamarin.Forms.Device.RuntimePlatform is no longer supported. Use Microsoft.Maui.Devices.DeviceInfo.Platform instead. For more details see https://learn.microsoft.com/en-us/dotnet/maui/migration/forms-projects#device-changes
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                insets = On<iOS>().SafeAreaInsets();
            }
            else if (OperatingSystem.IsAndroid())
            {
#if ANDROID
                insets = PlatformUtils.GetWindowSafeArea();
#endif
            }

            return proportion * (Height - insets.Top);
        }

        /**
         * <summary>
         * Normalize the requested sheet height so that it never exceeds the maximum.
         * </summary>
         * <param name="height">A positive number that will cap at the max height of the screen.</param>
         * */
        private double LimitYCoordToScreenMax(double height)
        {
            height = Math.Abs(height);

            var maxHeight = GetMaxHeight();
            if (height > maxHeight)
            {
                height = maxHeight;
            }

            return height;
        }

        private double GetMaxHeight()
        {
            return Math.Abs(GetProportionCoordinate(.95));
        }

        /**
         * <summary>Animates the sheet to a given Y-coordinate.</summary>
         * <param name="position">A negative value indicating the Y-coordinate to antimate to.</param>
         * <param name="easing">An optional Easing to control how the animation occurs. Defaults to `Easing.SpringOut`.</param>
         * <returns>A Task that will complete with the animation.</returns>
         */
        public async Task AnimateTo(double position, Easing easing = null)
        {
            easing ??= Easing.SpringOut;

            position = -LimitYCoordToScreenMax(position - Webview.KeyboardHeight);

            // Ignore small, negative adjustments in height
            var heightDifference = Math.Abs(Math.Abs(position) - Math.Abs(currentPosition));
            if (heightDifference < 50)
            {
                return;
            }

            detentPoint = position;

            await Sheet.TranslateTo(Sheet.X, position, duration, easing);
            currentPosition = Sheet.TranslationY;
        }

        private async Task AnimateIn()
        {
            await Task.WhenAll(
                Backdrop.FadeTo(0.5, length: duration),
                Sheet.TranslateTo(0, -InitialPosition, length: duration, easing: Easing.SpringOut),
                Sheet.FadeTo(1, duration, Easing.SpringOut)
            );
            currentPosition = Sheet.TranslationY;
        }

        private async Task AnimateOut()
        {
            await Task.WhenAll(
                Sheet.TranslateTo(x: 0, y: 0, length: duration, easing: Easing.SinIn),
                Sheet.FadeTo(0, duration, Easing.SinIn),
                Backdrop.FadeTo(0, duration)
            );
            currentPosition = Sheet.TranslationY;
        }

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            _ = Dismiss();
        }

        private async Task Shake()
        {
            uint timeout = 50;
            await Sheet.TranslateTo(0, currentPosition - 15, timeout);
            await Sheet.TranslateTo(0, currentPosition + 15, timeout);
            await Sheet.TranslateTo(0, currentPosition - 10, timeout);
            await Sheet.TranslateTo(0, currentPosition + 10, timeout);
            await Sheet.TranslateTo(0, currentPosition - 5, timeout);
            await Sheet.TranslateTo(0, currentPosition + 5, timeout);
            Sheet.TranslationY = currentPosition;
        }
    }
}