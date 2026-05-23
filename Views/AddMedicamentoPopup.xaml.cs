using CommunityToolkit.Maui.Views;
using EnvejecerConBienestar.Models;

namespace EnvejecerConBienestar.Views;

public partial class AddMedicamentoPopup : Popup
{
    private readonly List<Sugerencia> _sugerencias;
    private Frame? _tarjetaSeleccionada;

    public TaskCompletionSource<Medicamento?> PopupResult { get; } = new();

    public AddMedicamentoPopup()
    {
        InitializeComponent();

        _sugerencias = new List<Sugerencia>
        {
            new("💊", "Acetaminofén",  "500", 8),
            new("💊", "Ibuprofeno",    "400", 8),
            new("💊", "Metformina",    "500", 12),
            new("💊", "Losartán",      "50",  24),
            new("💊", "Omeprazol",     "20",  24),
            new("💉", "Insulina",      "10",  12),
            new("💊", "Atorvastatina", "10",  24),
            new("💊", "Levotiroxina",  "100", 24),
        };

        ConstruirTarjetasSugerencias();
    }

    private void ConstruirTarjetasSugerencias()
    {
        foreach (var sug in _sugerencias)
        {
            var tarjeta = CrearTarjetaSugerencia(sug);
            SugerenciasContainer.Children.Add(tarjeta);
        }
    }

    private Frame CrearTarjetaSugerencia(Sugerencia sug)
    {
        var icono = new Label
        {
            Text = sug.Icono,
            FontSize = 24,
            HorizontalOptions = LayoutOptions.Center
        };

        var nombre = new Label
        {
            Text = sug.Nombre,
            FontFamily = "NunitoBold",
            FontSize = 13,
            TextColor = Color.FromArgb("#1E293B"),
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 2,
            MaximumWidthRequest = 100
        };

        var dosis = new Label
        {
            Text = $"{sug.Miligramos} mg · Cada {sug.Frecuencia}h",
            FontFamily = "NunitoRegular",
            FontSize = 11,
            TextColor = Color.FromArgb("#64748B"),
            HorizontalOptions = LayoutOptions.Center
        };

        var contenido = new VerticalStackLayout
        {
            Spacing = 6,
            Padding = new Thickness(10, 12),
            HorizontalOptions = LayoutOptions.Center,
            Children = { icono, nombre, dosis }
        };

        var tarjeta = new Frame
        {
            Content = contenido,
            BackgroundColor = Color.FromArgb("#F8FAFC"),
            CornerRadius = 16,
            Padding = 0,
            HasShadow = false,
            BorderColor = Color.FromArgb("#E2E8F0"),
            WidthRequest = 130,
            HeightRequest = 120
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += (s, e) => OnSugerenciaTapped(tarjeta, sug);
        tarjeta.GestureRecognizers.Add(tap);

        return tarjeta;
    }

    private void OnSugerenciaTapped(Frame tarjeta, Sugerencia sug)
    {
        // Restaurar color de la tarjeta previamente seleccionada
        if (_tarjetaSeleccionada is not null)
        {
            _tarjetaSeleccionada.BackgroundColor = Color.FromArgb("#F8FAFC");
            _tarjetaSeleccionada.BorderColor = Color.FromArgb("#E2E8F0");
        }

        // Marcar la nueva tarjeta como seleccionada
        tarjeta.BackgroundColor = Color.FromArgb("#CCFBF1");    // ColorPrimarioClaro
        tarjeta.BorderColor = Color.FromArgb("#0D9488");        // ColorPrimario
        _tarjetaSeleccionada = tarjeta;

        // Pre-rellenar el formulario automáticamente
        NombreEntry.Text = sug.Nombre;
        MgEntry.Text = sug.Miligramos;
        FrecuenciaEntry.Text = sug.Frecuencia.ToString();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        PopupResult.SetResult(null);
        Close();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreEntry.Text))
        {
            await Shell.Current.CurrentPage.DisplayAlert(
                "Falta un detalle",
                "Por favor, dinos el nombre del medicamento para poder cuidarte mejor. 🌿",
                "Entendido");
            return;
        }

        var med = new Medicamento
        {
            Nombre = NombreEntry.Text.Trim(),
            Miligramos = MgEntry.Text?.Trim() ?? "",
            Frecuencia = int.TryParse(FrecuenciaEntry.Text?.Trim(), out int f) ? f : 8,
            HoraAlarma = TimePicker.Time,
            FechaInicio = DateTime.Now
        };

        // Mensaje cálido antes de cerrar
        await Shell.Current.CurrentPage.DisplayAlert(
            "¡Excelente! 🌸",
            $"Hemos anotado tu {med.Nombre}. No te preocupes, yo me encargo de recordártelo.",
            "¡Gracias!");

        PopupResult.SetResult(med);
        Close();
    }

    private sealed record Sugerencia(
        string Icono,
        string Nombre,
        string Miligramos,
        int Frecuencia);
}
