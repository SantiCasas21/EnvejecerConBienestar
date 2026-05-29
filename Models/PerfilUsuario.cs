using SQLite;

namespace EnvejecerConBienestar.Models;

public class PerfilUsuario : IEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Nombre { get; set; } = string.Empty;

    public int Edad { get; set; }

    public string TipoSangre { get; set; } = string.Empty;

    public string Alergias { get; set; } = string.Empty;

    public string Condiciones { get; set; } = string.Empty;

    public string Telefono { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    [Ignore]
    public bool EstaCompleto => !string.IsNullOrWhiteSpace(Nombre);

    [Ignore]
    public string TextoEdad => Edad > 0 ? $"{Edad} años" : "No especificada";

    [Ignore]
    public string TextoTipoSangre => string.IsNullOrWhiteSpace(TipoSangre) ? "No especificado" : TipoSangre;

    [Ignore]
    public string TextoAlergias => string.IsNullOrWhiteSpace(Alergias) ? "No registradas" : Alergias;

    [Ignore]
    public string TextoCondiciones => string.IsNullOrWhiteSpace(Condiciones) ? "No registradas" : Condiciones;
}
