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
                await Shell.Current.DisplayAlert("Error", "El marcado telefónico no está soportado.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo realizar la llamada: {ex.Message}", "OK");
        }
    }
}
