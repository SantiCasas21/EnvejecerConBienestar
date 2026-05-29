using EnvejecerConBienestar.Models;
using Plugin.LocalNotification;

namespace EnvejecerConBienestar.Services;

public class AlarmService
{
    private int _notificationId = 1000;

    public async Task ProgramarAlarma(Medicamento medicamento)
    {
        await CancelarAlarmasDeMedicamento(medicamento);

        if (medicamento.Frecuencia <= 0)
            return;

        var nombreUsuario = Preferences.Get("nombre_usuario", string.Empty);
        var saludo = string.IsNullOrWhiteSpace(nombreUsuario) ? "Hola" : $"Hola {nombreUsuario}";

        var horarios = medicamento.HorarioDiario;
        foreach (var horario in horarios)
        {
            var notifId = _notificationId++;

            var notifyTime = DateTimeOffset.Now.Date.Add(horario);
            if (notifyTime <= DateTimeOffset.Now)
                notifyTime = notifyTime.AddDays(1);

            var notification = new NotificationRequest
            {
                NotificationId = notifId,
                Title = "Envejecer Con Bienestar",
                Description = $"{saludo}, es hora de tomar tu {medicamento.Nombre}. ¡Tu salud es lo primero! 🌸",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime,
                    NotifyRepeatInterval = TimeSpan.FromHours(medicamento.Frecuencia),
                },
#if ANDROID
                Android = new Plugin.LocalNotification.AndroidOption.AndroidOptions
                {
                    ChannelId = "medicacion_diaria",
                }
#endif
            };

            await LocalNotificationCenter.Current.Show(notification);
        }
    }

    public async Task CancelarAlarmasDeMedicamento(Medicamento medicamento)
    {
        var notificaciones = await LocalNotificationCenter.Current.GetPendingNotificationList();
        if (notificaciones == null || notificaciones.Count == 0) return;

        var idsACancelar = new List<int>();
        foreach (var n in notificaciones)
        {
            if (n.Title == "Envejecer Con Bienestar" &&
                n.Description != null &&
                n.Description.Contains(medicamento.Nombre))
            {
                idsACancelar.Add(n.NotificationId);
            }
        }

        foreach (var id in idsACancelar)
        {
            LocalNotificationCenter.Current.Cancel(id);
        }

        await Task.CompletedTask;
    }

    public static async Task InicializarCanal()
    {
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }
    }
}
