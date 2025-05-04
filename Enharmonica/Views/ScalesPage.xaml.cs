using Enharmonica.ViewModels;

namespace Enharmonica.Views;

public partial class ScalesPage : ContentPage
{
	public ScalesPage(ScalesPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}