using SQLite;

namespace EnvejecerConBienestar.Models;

public class Medicamento
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Nombre { get; set; } = string.Empty;

    public string Miligramos { get; set; } = string.Empty;

    public string Notas { get; set; } = string.Empty;

    public int Frecuencia { get; set; } // En horas

    public DateTime FechaInicio { get; set; }

    public TimeSpan HoraAlarma { get; set; }

    public bool EstaTomado { get; set; } = false;

    public string Icono { get; set; } = "💊";

    [Ignore]
    public string TextoBoton => EstaTomado ? "✓ Tomado" : "Marcar tomado";
}
