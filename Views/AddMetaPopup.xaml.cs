using CommunityToolkit.Maui.Views;
using EnvejecerConBienestar.Models;

namespace EnvejecerConBienestar.Views;

public partial class AddMetaPopup : Popup
{
    public TaskCompletionSource<Meta?> PopupResult { get; } = new();

    public AddMetaPopup()
    {
        InitializeComponent();
    }

    private void OnSugerenciaClicked(object sender, EventArgs e)
    {
        if (sender is not Button btn) return;

        var texto = btn.Text;
        // El formato es "icono Nombre", ej: "💧 Agua"
        var partes = texto.Split(' ', 2);
        var icono = partes.Length > 0 ? partes[0] : "🎯";
        var nombre = partes.Length > 1 ? partes[1] : texto;

        NombreEntry.Text = nombre;

        // Pre-llenar objetivo y unidad según el tipo
        (string objetivo, string unidad) = nombre.ToLower() switch
        {
            "agua" => ("8", "vasos"),
            "caminata" => ("30", "minutos"),
            "ejercicio" => ("1", "sesión"),
            "lectura" => ("20", "páginas"),
            "meditación" => ("10", "minutos"),
            "socializar" => ("1", "llamada"),
            "medicinas" => ("1", "dosis"),
            "sueño" => ("8", "horas"),
            _ => ("1", "veces")
        };
        ObjetivoEntry.Text = objetivo;
        UnidadEntry.Text = unidad;
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        PopupResult.SetResult(null);
        Close();
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreEntry.Text)) return;
        if (!int.TryParse(ObjetivoEntry.Text, out int objetivo) || objetivo <= 0) return;

        var ahora = DateTime.Now;
        var frecuencia = RadioDiario.IsChecked ? "Diaria" : "Semanal";
        var fechaFin = frecuencia == "Diaria"
            ? ahora.Date.AddDays(1).AddSeconds(-1)
            : ahora.Date.AddDays(7).AddSeconds(-1);

        var meta = new Meta
        {
            Nombre = NombreEntry.Text.Trim(),
            Objetivo = objetivo,
            Progreso = 0,
            Unidad = UnidadEntry.Text?.Trim() ?? "veces",
            Frecuencia = frecuencia,
            FechaInicio = ahora,
            FechaFin = fechaFin,
            Icono = ObtenerIcono(NombreEntry.Text.Trim()),
            ColorIcono = ObtenerColorIcono(NombreEntry.Text.Trim())
        };

        PopupResult.SetResult(meta);
        Close();
    }

    private static string ObtenerIcono(string nombre)
    {
        return nombre.ToLower() switch
        {
            "agua" or "hidratación" => "💧",
            "caminata" or "caminar" => "🚶",
            "ejercicio" or "actividad física" => "💪",
            "lectura" or "leer" => "📖",
            "meditación" or "respiración" => "🧘",
            "socializar" or "compañía" => "🤝",
            "medicinas" or "medicamentos" => "💊",
            "sueño" or "descanso" => "🌙",
            _ => "🎯"
        };
    }

    private static string ObtenerColorIcono(string nombre)
    {
        return nombre.ToLower() switch
        {
            "agua" or "hidratación" => "#0284C7",
            "caminata" or "caminar" => "#22C55E",
            "ejercicio" or "actividad física" => "#818CF8",
            "lectura" or "leer" => "#F97316",
            "meditación" or "respiración" => "#7C3AED",
            "socializar" or "compañía" => "#E11D48",
            "medicinas" or "medicamentos" => "#64748B",
            "sueño" or "descanso" => "#6366F1",
            _ => "#0D9488"
        };
    }
}
