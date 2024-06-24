using System;
using System.Threading.Tasks;
using ReduxSimple;
using Rownd.Maui.Core;
using Rownd.Maui.Models.Domain;
using Rownd.Maui.Models.Repos;
using Rownd.Maui.Utils;
using static Rownd.Maui.RowndInstance;

namespace Rownd.Maui
{
    public interface IRowndInstance
    {
        void RequestSignIn();
        void RequestSignIn(SignInMethod with);
        void RequestSignIn(SignInOptions opts);
        void SignOut();
        void ManageAccount();
        void ManageAccount(RowndManageAccountOpts opts);
        Task<string> GetAccessToken();
        Task<string> GetAccessToken(string token);
        Task<string> GetAccessToken(RowndTokenOpts opts);
        ReduxStore<GlobalState> Store { get; }
        UserRepo User { get; }
#pragma warning disable SA1300 // Element should begin with upper-case letter
        Task _InternalTestRefreshToken();
#pragma warning restore SA1300 // Element should begin with upper-case letter

        event EventHandler<RowndEventArgs> Events;
    }
}
