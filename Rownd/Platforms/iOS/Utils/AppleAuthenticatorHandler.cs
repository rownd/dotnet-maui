using System;
using Rownd.Maui.iOS;

namespace Rownd.Maui.Utils
{
    partial class AppleAuthenticatorHandler
    {
        private IAppleAuthCoordinator appleAuthCoordinator;

        internal partial void InitPlatformCode()
        {
            appleAuthCoordinator = new AppleAuthCoordinator();
            appleAuthCoordinator.Inject(rownd, authRepo);
        }

        internal partial void SignIn(SignInIntent? intent)
        {
            appleAuthCoordinator.SignIn(intent);
        }
    }
}
