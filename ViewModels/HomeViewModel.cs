using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace EnvejecerConBienestar.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    // ---- Propiedades ----

    [ObservableProperty]
    private string _nombreUsuario = "Rosa";

    [ObservableProperty]
    private string _saludo = string.Empty;

    [ObservableProperty]
    private string _fechaHoy = string.Empty;

    [ObservableProperty]
    private string _proximoMedicamento = "Metformina · 500mg";

    [ObservableProperty]
    private string _horaProximoMedicamento = "8:00 a.m.";

    [ObservableProperty]
    private string _iconoSaludo = "☀️";

    public HomeViewModel()
    {
        ActualizarSaludo();
    }

    // ---- Lógica de saludo por hora del día ----

    private void ActualizarSaludo()
    {
        var hora = DateTime.Now.Hour;

        if (hora >= 6 && hora < 12)
        {
            Saludo = $"¡Buenos días, {NombreUsuario}!";
            IconoSaludo = "☀️";
        }
        else if (hora >= 12 && hora < 19)
        {
            Saludo = $"¡Buenas tardes, {NombreUsuario}!";
            IconoSaludo = "🌤️";
        }
        else
        {
            Saludo = $"¡Buenas noches, {NombreUsuario}!";
            IconoSaludo = "🌙";
        }

        FechaHoy = DateTime.Now.ToString("dddd, d 'de' MMMM 'de' yyyy",
            new System.Globalization.CultureInfo("es-CO"));

        // Capitalizar primera letra
        if (FechaHoy.Length > 0)
            FechaHoy = char.ToUpper(FechaHoy[0]) + FechaHoy[1..];
    }

    // ---- Comando: Botón de emergencia ----

    [RelayCommand]
    private async Task LlamarEmergencia()
    {
        // En producción: PhoneDialer.Default.Open("112") o número configurado
        bool confirmar = await Shell.Current.DisplayAlert(
            "🚨 EMERGENCIA",
            "¿Deseas llamar al número de emergencia?\n\n" +
            "Se contactará a tu familiar de confianza y a los servicios de emergencia.",
            "SÍ, LLAMAR AHORA",
            "Cancelar");

        if (confirmar)
        {
            // Simulación para el prototipo
            await Shell.Current.DisplayAlert(
                "Llamando...",
                "📞 Conectando con Carmen López (hija)...\n\nEn la versión final se marcará el número real.",
                "OK");
        }
    }
}
