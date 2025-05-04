using Enharmonica.ViewModels;

namespace Enharmonica.Views;

public partial class ChordsPage : ContentPage
{
	public ChordsPage(ChordsPageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}