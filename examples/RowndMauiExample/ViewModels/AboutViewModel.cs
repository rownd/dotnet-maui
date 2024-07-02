using System;
using System.ComponentModel;
using System.Windows.Input;
using Rownd.Maui;
using Rownd.Maui.Models.Domain;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.ApplicationModel;

namespace RowndMauiExample.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        private readonly Page page;
        public GlobalState RowndState { get; set; }
        public string? FirstName
        {
            get
            {
                RowndState.User.Data.TryGetValue("first_name", out dynamic firstName);
                return firstName;
            }
        }

        public AboutViewModel(Page page)
        {
            this.page = page;

            Title = "About";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
            RequestSignIn = new Command(() => Rownd.RequestSignIn());
            SignOut = new Command(() => Rownd.SignOut());
            RefreshToken = new Command(() => Rownd._InternalTestRefreshToken());

            EditProfile = new Command(() => Rownd.ManageAccount(new Rownd.Maui.Utils.RowndManageAccountOpts
            {
                VisibleProfileFields = new string[] { "phone_number" },
                AutoFocusField = "phone_number"
            }));

            UpdateName = new Command(async () =>
            {
                RowndState.User.Data.TryGetValue("first_name", out var firstName);
                string result = await page.DisplayPromptAsync("Question 1", "What's your name?", initialValue: firstName);
                if (result != null)
                {
                    Rownd.User.Set("first_name", result);
                }
            });
            Rownd.Store.Select().Subscribe((state) =>
            {
                RowndState = state;
                OnPropertyChanged("RowndState");
                OnPropertyChanged("FirstName");
            });
        }

        public ICommand OpenWebCommand { get; }

        public ICommand RequestSignIn { get; }

        public ICommand SignOut { get; }

        public ICommand RefreshToken { get; }

        public ICommand UpdateName { get; }

        public ICommand EditProfile { get; }

        //public event PropertyChangedEventHandler PropertyChanged;
        //private void NotifyPropertyChanged(string property)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(property));
        //    }
        //}
    }
}
