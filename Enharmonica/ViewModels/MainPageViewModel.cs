using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Enharmonica.Services;
using Enharmonica.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enharmonica.ViewModels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        

        [ObservableProperty]
        private string _welcomeMessage = "ENHARMONICA";

        public MainPageViewModel(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
        }

        [RelayCommand]
        public async Task NavigateToScalesAsync()
        {
            var scalesPage = _serviceProvider.GetService<ScalesPage>();
            await Shell.Current.GoToAsync(nameof(ScalesPage));
        }

        [RelayCommand]
        public async Task NavigateToChordsAsync()
        {
            var chordsPage = _serviceProvider.GetService<ChordsPage>();
            await Shell.Current.GoToAsync(nameof(ChordsPage));
        }

        [RelayCommand]
        public async Task NavigateToMetronomeAsync()
        {
            var metronomePage = _serviceProvider.GetService<MetronomePage>();
            await Shell.Current.GoToAsync(nameof(MetronomePage));
        }

        [RelayCommand]
        public async Task NavigateToPitchDetectionAsync()
        {
            var pitchDetectionPage = _serviceProvider.GetService<PitchDetectionPage>();
            await Shell.Current.GoToAsync(nameof(PitchDetectionPage));
        }
    }
}
