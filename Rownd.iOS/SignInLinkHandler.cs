﻿using System.Threading.Tasks;
using Foundation;
using Rownd.Xamarin.Utils;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(Rownd.Xamarin.iOS.SignInLinkHandler))]
namespace Rownd.Xamarin.iOS
{
    public class SignInLinkHandler : ISignInLinkHandler
    {
        public SignInLinkHandler()
        {
        }

        public async Task<string> HandleSignInLink()
        {
            return await SignInWithLinkIfPresentOnLaunchOrClipboard();
        }

        public async Task<string> SignInWithLinkIfPresentOnLaunchOrClipboard()
        {
            var tcs = new TaskCompletionSource<string>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                if (!UIPasteboard.General.HasStrings)
                {
                    tcs.TrySetResult(null);
                    return;
                }

                var patterns = await UIPasteboard.General.DetectPatternsAsync(new NSSet<NSString>(new NSString("com.apple.uikit.pasteboard-detection-pattern.probable-web-url")));

                if (patterns.Count == 0)
                {
                    tcs.TrySetResult(null);
                    return;
                }

                if (!patterns.Contains("com.apple.uikit.pasteboard-detection-pattern.probable-web-url"))
                {
                    tcs.TrySetResult(null);
                    return;
                }

                var launchUrl = UIPasteboard.General.String;

                if (launchUrl != null && !launchUrl.ToLower().StartsWith("http"))
                {
                    launchUrl = $"https://{launchUrl}";
                }

                tcs.TrySetResult(launchUrl);
            });

            return await tcs.Task;
        }
    }
}
