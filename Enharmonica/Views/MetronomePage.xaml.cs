using Enharmonica.ViewModels;

namespace Enharmonica.Views;

public partial class MetronomePage : ContentPage
{
	public MetronomePage()
	{
		InitializeComponent();
        BindingContext = new MetronomePageViewModel();
    }
}