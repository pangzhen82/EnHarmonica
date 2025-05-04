using Enharmonica.ViewModels;

namespace Enharmonica.Views;

public partial class PitchDetectionPage : ContentPage
{
	public PitchDetectionPage()
	{
		InitializeComponent();
        BindingContext = new PitchDetectionPageViewModel();
    }
}