using System.Runtime.Serialization;

namespace Rownd.Maui.Utils
{
    public enum SignInIntent
    {
        [EnumMember(Value="sign_up")]
        SignUp,

        [EnumMember(Value = "sign_in")]
        SignIn
    }
}
