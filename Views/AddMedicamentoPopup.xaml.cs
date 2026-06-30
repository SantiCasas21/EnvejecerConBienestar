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
            new("\U0001F48A", "Acetaminofén",  "500", 8, "#0D9488"),
            new("\U0001F48A", "Ibuprofeno",    "400", 8, "#818CF8"),
            new("\U0001F48A", "Metformina",    "500", 12, "#64748B"),
            new("\U0001F48A", "Losartán",      "50",  24, "#0D9488"),
            new("\U0001F48A", "Omeprazol",     "20",  24, "#818CF8"),
            new("\U0001F489", "Insulina",      "10",  12, "#E11D48"),
            new("\U0001F48A", "Atorvastatina", "10",  24, "#64748B"),
            new("\U0001F48A", "Levotiroxina",  "100", 24, "#818CF8"),
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
            TextColor = Color.FromArgb(sug.Color),
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

        // Si hay una sugerencia seleccionada, heredar su icono y color
        if (_tarjetaSeleccionada != null)
        {
            var content = (VerticalStackLayout)_tarjetaSeleccionada.Content;
            var iconoLabel = (Label)content.Children[0];
            med.Icono = iconoLabel.Text;
            med.ColorIcono = iconoLabel.TextColor.ToHex();
        }

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
        int Frecuencia,
        string Color);
}
