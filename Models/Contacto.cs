using SQLite;

namespace EnvejecerConBienestar.Models;

public class Contacto
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Nombre { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Ubicacion { get; set; } = string.Empty;

    public string Categoria { get; set; } = "Familia/Amigos";

    public string Icono { get; set; } = "👤";

    public bool EsFavorito { get; set; }

    public bool EsEmergencia { get; set; }
}
