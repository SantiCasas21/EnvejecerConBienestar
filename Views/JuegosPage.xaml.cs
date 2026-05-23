namespace EnvejecerConBienestar.Views;

public partial class JuegosPage : ContentPage
{
    public JuegosPage()
    {
        InitializeComponent();
    }

    // Juego 1 — Buscar los pares
    private async void OnBuscarParesTocado(object sender, TappedEventArgs e)
        => await Navigation.PushAsync(new BuscarParesPage());

    // Juego 2 — Trivia de Salud
    private async void OnTriviaTocado(object sender, TappedEventArgs e)
        => await Navigation.PushAsync(new TriviaPage());

    // Juego 3 — Sopa de Letras
    private async void OnSopaLetrasTocado(object sender, TappedEventArgs e)
        => await Navigation.PushAsync(new SopaLetrasPage());
}
