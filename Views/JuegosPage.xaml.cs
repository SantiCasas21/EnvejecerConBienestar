namespace EnvejecerConBienestar.Views;

public partial class JuegosPage : ContentPage
{
    public JuegosPage()
    {
        InitializeComponent();
    }

    private async void OnBuscarParesTocado(object sender, TappedEventArgs e)
        => await Navigation.PushAsync(new BuscarParesPage());

    private async void OnTriviaTocado(object sender, TappedEventArgs e)
        => await Navigation.PushAsync(new TriviaPage());

    private async void OnSopaLetrasTocado(object sender, TappedEventArgs e)
        => await Navigation.PushAsync(new SopaLetrasPage());

    private async void OnOrdenarSecuenciaTocado(object sender, TappedEventArgs e)
        => await Navigation.PushAsync(new OrdenarSecuenciaPage());
}
