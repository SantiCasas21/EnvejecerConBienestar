using CommunityToolkit.Maui.Views;
using EnvejecerConBienestar.Models;

namespace EnvejecerConBienestar.Views;

public partial class BienvenidaPopup : Popup
{
    public TaskCompletionSource<PerfilUsuario?> Resultado { get; } = new();

    public BienvenidaPopup()
    {
        InitializeComponent();
        Opened += OnPopupOpened;
    }

    private async void OnPopupOpened(object? sender, EventArgs e)
    {
        await Task.Delay(300);
        NombreEntry.Focus();
    }

    private void OnComenzarClicked(object sender, EventArgs e)
    {
        var nombre = NombreEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            NombreEntry.Placeholder = "Por favor, escribe tu nombre...";
            NombreEntry.Focus();
            return;
        }

        var perfil = new PerfilUsuario
        {
            Nombre = nombre
        };

        if (int.TryParse(EdadEntry.Text?.Trim(), out var edad))
            perfil.Edad = edad;

        perfil.TipoSangre = SangreEntry.Text?.Trim() ?? string.Empty;
        perfil.Telefono = TelefonoEntry.Text?.Trim() ?? string.Empty;

        Resultado.SetResult(perfil);
        Close();
    }
}
