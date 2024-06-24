using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RowndMauiExample.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public Command LoginCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool isOpenSimple = false;
        public bool IsOpenSimple
        {
            get => isOpenSimple;
            set => SetAndRaisePropertyChanged(ref isOpenSimple, value);
        }

        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }

        private void OnLoginClicked(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            //await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
            isOpenSimple = true;
        }

        protected void SetAndRaisePropertyChanged<TRef>(
            ref TRef field, TRef value, [CallerMemberName] string propertyName = null)
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}