namespace EnvejecerConBienestar.Views;

public partial class JuegosPage : ContentPage
{
    public JuegosPage()
    {
        InitializeComponent();
    }

    // Navega al juego "Buscar los pares"
    private async void OnBuscarParesTocado(object sender, TappedEventArgs e)
        => await Navigation.PushAsync(new BuscarParesPage());

    // Navega al juego "Trivia de Salud"
    private async void OnTriviaTocado(object sender, TappedEventArgs e)
        => await Navigation.PushAsync(new TriviaPage());
}
