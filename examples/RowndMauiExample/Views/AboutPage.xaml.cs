using System;
using System.ComponentModel;
using RowndMauiExample.ViewModels;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RowndMauiExample.Views
{
    public partial class AboutPage : ContentPage
    {
        private AboutViewModel ViewModel;

        public AboutPage()
        {
            InitializeComponent();
            ViewModel = new(this);
            BindingContext = ViewModel;
        }
    }
}
