using SQLite;

namespace EnvejecerConBienestar.Models;

public class ActividadCognitiva : IEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string TipoJuego { get; set; } = string.Empty;

    public int Puntaje { get; set; }

    public DateTime FechaRealizacion { get; set; }
}
