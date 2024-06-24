using RowndMauiExample.ViewModels;

namespace RowndMauiExample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private LoginViewModel ViewModel;

        public LoginPage()
        {
            InitializeComponent();
            ViewModel = new();
            BindingContext = ViewModel;
        }
    }
}
