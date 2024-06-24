using System;
using Rownd.Maui.Utils;

namespace Rownd.Maui.Android
{
    public class Boot
    {
        public static void Init()
        {
            DependencyService.Register<ISignInLinkHandler, SignInLinkHandler>();
            //DependencyService.Register<IPlatformUtils, PlatformUtils>();
        }
    }
}
