namespace EnvejecerConBienestar.Models;

/// <summary>
/// Representa un medicamento en la agenda del usuario.
/// </summary>
public class Medicamento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Dosis { get; set; } = string.Empty;
    public string Hora { get; set; } = string.Empty;
    public string Icono { get; set; } = "💊";
    public bool Tomado { get; set; } = false;

    /// <summary>
    /// Texto descriptivo para el botón según el estado.
    /// </summary>
    public string TextoBoton => Tomado ? "✓ Tomado" : "Marcar tomado";
}
