using System;
using System.Text;
using Newtonsoft.Json;

namespace Rownd.Maui.Models.Domain
{
    public class SignInState
    {
        public string LastSignIn { get; set; } = null;
        public string LastSignInDate { get; set; } = null;

        public string ToSignInInitHash()
        {
            var json = JsonConvert.SerializeObject(this);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json), Base64FormattingOptions.None);
        }
    }
}
