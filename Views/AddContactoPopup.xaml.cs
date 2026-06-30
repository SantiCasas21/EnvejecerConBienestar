using CommunityToolkit.Maui.Views;
using EnvejecerConBienestar.Models;

namespace EnvejecerConBienestar.Views;

public partial class AddContactoPopup : Popup
{
    public TaskCompletionSource<Contacto?> PopupResult { get; } = new();

    public AddContactoPopup()
    {
        InitializeComponent();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        PopupResult.SetResult(null);
        Close();
    }

    private void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NombreEntry.Text))
        {
            Shell.Current.DisplayAlert("Falta el nombre", "El nombre del contacto es obligatorio.", "OK");
            return;
        }

        var contacto = new Contacto
        {
            Nombre = NombreEntry.Text.Trim(),
            Relacion = RelacionEntry.Text?.Trim() ?? "",
            Telefono = TelefonoEntry.Text?.Trim() ?? "",
            Ubicacion = UbicacionEntry.Text?.Trim() ?? "",
            EsFavorito = FavoritoCheck.IsChecked,
            EsEmergencia = EmergenciaCheck.IsChecked,
            Icono = "👤",
            ColorAvatar = Contacto.GenerarColorPorNombre(NombreEntry.Text.Trim())
        };

        PopupResult.SetResult(contacto);
        Close();
    }
}
