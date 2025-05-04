using Enharmonica.Views;

namespace Enharmonica
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ScalesPage), typeof(ScalesPage));
            Routing.RegisterRoute(nameof(ChordsPage), typeof(ChordsPage));
            Routing.RegisterRoute(nameof(MetronomePage), typeof(MetronomePage));
            Routing.RegisterRoute(nameof(PitchDetectionPage), typeof(PitchDetectionPage));
        }
    }
}
