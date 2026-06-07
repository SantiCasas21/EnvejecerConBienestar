using SQLite;

namespace EnvejecerConBienestar.Models;

public class Medicamento : IEntity
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

    public string Icono { get; set; } = "\U0001F48A"; // pill emoji

    public string ColorIcono { get; set; } = "#0D9488"; // ColorPrimario

    public int CantidadRestante { get; set; }

    public int UmbralAlerta { get; set; } = 5;

    [Ignore]
    public string TextoBoton => EstaTomado ? "✅  Tomado" : "Marcar tomado";

    [Ignore]
    public string IndicadorEstado => EstaTomado ? "✅" : "⏳";

    [Ignore]
    public bool AlertaInventario => CantidadRestante > 0 && CantidadRestante <= UmbralAlerta;

    [Ignore]
    public string TextoInventario => CantidadRestante > 0
        ? $"Quedan {CantidadRestante} pastillas"
        : "Sin control de inventario";

    [Ignore]
    public Color ColorInventario => AlertaInventario
        ? Color.FromArgb("#E11D48")
        : Color.FromArgb("#64748B");

    [Ignore]
    public List<TimeSpan> HorarioDiario => CalcularHorarioDiario();

    [Ignore]
    public string TextoHorario => ObtenerTextoHorario();

    [Ignore]
    public int TomasPorDia => HorarioDiario.Count;

    private List<TimeSpan> CalcularHorarioDiario()
    {
        var horarios = new List<TimeSpan>();
        if (Frecuencia <= 0) Frecuencia = 24;

        var horaActual = HoraAlarma;
        while (horaActual.TotalHours < 24)
        {
            horarios.Add(horaActual);
            horaActual = horaActual.Add(TimeSpan.FromHours(Frecuencia));
        }

        return horarios;
    }

    private string ObtenerTextoHorario()
    {
        var horarios = HorarioDiario;
        if (!horarios.Any()) return "Sin horario";

        return string.Join("\n", horarios.Select((h, i) =>
            $"  Toma {i + 1}: {h:hh\\:mm}"));
    }
}
