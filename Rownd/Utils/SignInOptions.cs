using System;
namespace Rownd.Maui.Utils
{
    public class SignInOptions
    {
        public string PostSignInRedirect { get; set; } = "NATIVE_APP";
        public SignInIntent Intent { get; set; }
    }
}
