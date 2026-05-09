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
        if (string.IsNullOrWhiteSpace(NombreEntry.Text)) return;

        var contacto = new Contacto
        {
            Nombre = NombreEntry.Text,
            Telefono = TelefonoEntry.Text ?? "",
            Ubicacion = UbicacionEntry.Text ?? "",
            EsFavorito = FavoritoCheck.IsChecked,
            EsEmergencia = EmergenciaCheck.IsChecked,
            Icono = "👤"
        };

        PopupResult.SetResult(contacto);
        Close();
    }
}
