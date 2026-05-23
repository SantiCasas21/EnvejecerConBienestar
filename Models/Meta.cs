using SQLite;

namespace EnvejecerConBienestar.Models;

public class Meta
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Nombre { get; set; } = string.Empty;

    public string Icono { get; set; } = "🎯";

    public int Objetivo { get; set; }

    public int Progreso { get; set; }

    public string Unidad { get; set; } = "veces";

    public string Frecuencia { get; set; } = "Diaria";

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public bool Completada { get; set; }

    [Ignore]
    public double Porcentaje => Objetivo > 0 ? Math.Min(1.0, (double)Progreso / Objetivo) : 0;

    [Ignore]
    public string TextoProgreso => $"{Progreso} / {Objetivo} {Unidad}";

    [Ignore]
    public string MensajeExito => ObtenerMensajeExito();

    [Ignore]
    public string Recomendacion => ObtenerRecomendacion();

    private string ObtenerMensajeExito()
    {
        return Nombre.ToLower() switch
        {
            "agua" or "hidratación" => "¡Excelente! Mantenerse hidratado es clave para tu salud. 🌊",
            "caminata" or "caminar" => "¡Maravilloso! Cada paso cuenta para un corazón fuerte. 🚶",
            "ejercicio" or "actividad física" => "¡Fantástico! El ejercicio es la mejor medicina. 💪",
            "lectura" or "leer" => "¡Bien hecho! Leer mantiene tu mente activa y despierta. 📚",
            "meditación" or "respiración" => "¡Perfecto! La calma interior es un gran tesoro. 🧘",
            "medicamentos" or "medicinas" => "¡Muy responsable! Cuidar tu salud es lo primero. 💊",
            "socializar" or "compañía" => "¡Qué bonito! Las relaciones nos mantienen vivos. 🤝",
            "sueño" or "descanso" => "¡Buen trabajo! Un buen descanso renueva cuerpo y mente. 😴",
            _ => $"¡Increíble logro! Has completado tu meta de {Nombre.ToLower()}. ¡Sigue así! 🌟"
        };
    }

    private string ObtenerRecomendacion()
    {
        return Nombre.ToLower() switch
        {
            "agua" or "hidratación" => "Ten un vaso grande de agua siempre a la vista para recordar beber.",
            "caminata" or "caminar" => "Empieza con 5 minutos. Una caminata corta es mejor que ninguna.",
            "ejercicio" or "actividad física" => "Prueba estiramientos suaves desde tu silla. Cada movimiento cuenta.",
            "lectura" or "leer" => "Lee aunque sea una página al día. La constancia es la clave.",
            "meditación" or "respiración" => "Respira profundo 3 veces ahora mismo. Solo toma un minuto.",
            "medicamentos" or "medicinas" => "Configura una alarma en la sección Medicinas de esta app.",
            "socializar" or "compañía" => "Una llamada corta a un ser querido puede alegrar tu día.",
            "sueño" or "descanso" => "Apaga las pantallas 30 minutos antes de dormir. Tu cuerpo lo agradecerá.",
            _ => $"Para alcanzar tu meta de {Nombre.ToLower()}, intenta avanzar poco a poco. ¡Cada pequeño paso cuenta!"
        };
    }
}
