using EnvejecerConBienestar.Views;

namespace EnvejecerConBienestar;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Views.BuscarParesPage), typeof(Views.BuscarParesPage));
        Routing.RegisterRoute(nameof(Views.TriviaPage), typeof(Views.TriviaPage));
        Routing.RegisterRoute(nameof(BuscarParesPage), typeof(BuscarParesPage));
        Routing.RegisterRoute(nameof(Views.ContactoDetailPage), typeof(Views.ContactoDetailPage));
        Routing.RegisterRoute(nameof(Views.MedicamentoDetailPage), typeof(Views.MedicamentoDetailPage));
    }
}
