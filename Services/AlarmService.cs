using EnvejecerConBienestar.Models;

namespace EnvejecerConBienestar.Services;

public class AlarmService
{
    public async Task ProgramarAlarma(Medicamento medicamento)
    {
        // En una implementación real de Android usaríamos AlarmManager vía DependencyService o Interface.
        // Para este prototipo, simulamos la programación.
        
        string mensaje = $"Alarma programada para {medicamento.Nombre} a las {medicamento.HoraAlarma:hh\\:mm}.";
        
#if ANDROID
        // Aquí iría la lógica de Intent para Android
        // var intent = new Android.Content.Intent(Android.Provider.AlarmClock.ActionSetAlarm);
        // intent.PutExtra(Android.Provider.AlarmClock.ExtraHour, medicamento.HoraAlarma.Hours);
        // ...
#endif

        await Shell.Current.DisplayAlert("Reloj del Sistema", mensaje, "Entendido");
    }
}
