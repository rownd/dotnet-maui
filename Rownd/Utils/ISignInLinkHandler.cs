using System;
using System.Threading.Tasks;

namespace Rownd.Maui.Utils
{
    public interface ISignInLinkHandler
    {
        public Task<string> HandleSignInLink();
    }
}
