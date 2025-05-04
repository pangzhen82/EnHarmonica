using Enharmonica.ViewModels;

namespace Enharmonica.Views
{
    public partial class MainPage : ContentPage
    { 
        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            //this.WidthRequest = 400;
            //this.HeightRequest = 700;
        }
    }
}
