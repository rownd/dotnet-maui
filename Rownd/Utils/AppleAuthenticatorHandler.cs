using System;
using Rownd.Maui.Models.Repos;

namespace Rownd.Maui.Utils
{
    internal partial class AppleAuthenticatorHandler
    {
        private readonly RowndInstance rownd;
        private readonly AuthRepo authRepo;

        internal partial void SignIn(SignInIntent? intent);
        internal partial void InitPlatformCode();

#if !IOS && !MACCATALYST
        internal partial void SignIn(SignInIntent? intent) => throw null!;
        internal partial void InitPlatformCode() => throw null!;
#endif

        public AppleAuthenticatorHandler(RowndInstance rownd, AuthRepo authRepo)
        {
            this.rownd = rownd;
            this.authRepo = authRepo;
        }
    }
}
