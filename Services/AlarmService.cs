using EnvejecerConBienestar.Models;

namespace EnvejecerConBienestar.Services;

public class AlarmService
{
    public async Task ProgramarAlarma(Medicamento medicamento)
    {
        // Para este prototipo, simulamos la programación sin interrumpir al usuario.
        await Task.CompletedTask;
    }
}
