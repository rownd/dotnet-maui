using System.ComponentModel;
using RowndMauiExample.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace RowndMauiExample.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}
