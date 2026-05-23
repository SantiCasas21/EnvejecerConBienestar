using SQLite;

namespace EnvejecerConBienestar.Models;

public class Contacto
{
    private static readonly string[] PaletaColores =
    {
        "#F97316", "#0D9488", "#7C3AED", "#1D4ED8", "#DC2626",
        "#059669", "#D97706", "#4F46E5", "#BE185D", "#0284C7"
    };

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Nombre { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public string Ubicacion { get; set; } = string.Empty;

    public string Relacion { get; set; } = string.Empty;

    public string Icono { get; set; } = "👤";

    public string ColorAvatar { get; set; } = string.Empty;

    public bool EsFavorito { get; set; }

    public bool EsEmergencia { get; set; }

    [Ignore]
    public string Inicial => string.IsNullOrWhiteSpace(Nombre)
        ? "?"
        : Nombre.TrimStart()[0].ToString().ToUpper();

    [Ignore]
    public string ColorAvatarFinal => string.IsNullOrWhiteSpace(ColorAvatar)
        ? GenerarColorPorNombre(Nombre)
        : ColorAvatar;

    public static string GenerarColorPorNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre)) return PaletaColores[0];
        int hash = Math.Abs(nombre.GetHashCode());
        return PaletaColores[hash % PaletaColores.Length];
    }
}
