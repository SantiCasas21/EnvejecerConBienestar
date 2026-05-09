namespace EnvejecerConBienestar;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Views.ContactoDetailPage), typeof(Views.ContactoDetailPage));
        Routing.RegisterRoute(nameof(Views.MedicamentoDetailPage), typeof(Views.MedicamentoDetailPage));
    }
}
