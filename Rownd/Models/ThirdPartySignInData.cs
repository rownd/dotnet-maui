using System;
using System.Collections.Generic;
using Rownd.Maui.Core;
using Rownd.Maui.Utils;

namespace Rownd.Maui.Models
{
    public class ThirdPartySignInData
    {
        public string Token { get; set; }
        public SignInIntent? Intent { get; set; }
        public SignInMethod SignInMethod { get; set; }
        public Dictionary<string, string> UserData { get; set; }
    }
}
