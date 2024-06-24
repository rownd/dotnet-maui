using System;
using Rownd.Maui.Models.Repos;

namespace Rownd.Maui.Utils
{
    public interface IAppleAuthCoordinator
    {
        public void Inject(RowndInstance rownd, AuthRepo authRepo);
        public void SignIn();
        public void SignIn(SignInIntent? intent);
    }
}
