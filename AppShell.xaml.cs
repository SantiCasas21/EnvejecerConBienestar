using EnvejecerConBienestar.Views;

namespace EnvejecerConBienestar;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        
        Routing.RegisterRoute(nameof(BuscarParesPage), typeof(BuscarParesPage));
        Routing.RegisterRoute(nameof(TriviaPage), typeof(TriviaPage));
        Routing.RegisterRoute(nameof(SopaLetrasPage), typeof(SopaLetrasPage));
        Routing.RegisterRoute(nameof(OrdenarSecuenciaPage), typeof(OrdenarSecuenciaPage));

        
        Routing.RegisterRoute(nameof(ContactoDetailPage), typeof(ContactoDetailPage));
        Routing.RegisterRoute(nameof(MedicamentoDetailPage), typeof(MedicamentoDetailPage));
    }
}
