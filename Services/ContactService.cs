namespace EnvejecerConBienestar.Services;

public class ContactService
{
    public async Task RealizarLlamada(string telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono)) return;

        try
        {
            if (PhoneDialer.Default.IsSupported)
                PhoneDialer.Default.Open(telefono);
            else
                await Shell.Current.DisplayAlert("Error", "El marcado telefónico no está soportado en este dispositivo.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo realizar la llamada: {ex.Message}", "OK");
        }
    }

    public async Task EnviarMensaje(string telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono)) return;

        try
        {
            if (Sms.Default.IsComposeSupported)
            {
                var message = new SmsMessage(string.Empty, telefono);
                await Sms.Default.ComposeAsync(message);
            }
            else
                await Shell.Current.DisplayAlert("Error", "El envío de mensajes no está soportado en este dispositivo.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo enviar el mensaje: {ex.Message}", "OK");
        }
    }
}
