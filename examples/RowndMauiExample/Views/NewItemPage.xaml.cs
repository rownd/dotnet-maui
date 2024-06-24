using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Maui.Controls.Xaml;

using RowndMauiExample.Models;
using RowndMauiExample.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RowndMauiExample.Views
{
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}
