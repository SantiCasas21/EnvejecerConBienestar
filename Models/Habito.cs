using SQLite;

namespace EnvejecerConBienestar.Models;

public class Habito : IEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Tipo { get; set; } = string.Empty; // "Agua", "Caminata", "Ejercicio"

    public int Meta { get; set; }

    public int ProgresoActual { get; set; }

    public DateTime Fecha { get; set; }

    [Ignore]
    public double Porcentaje => Meta > 0 ? (double)ProgresoActual / Meta : 0;
}
