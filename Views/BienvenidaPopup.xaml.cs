using CommunityToolkit.Maui.Views;

namespace EnvejecerConBienestar.Views;

public partial class BienvenidaPopup : Popup
{
    public TaskCompletionSource<string> Resultado { get; } = new();

    public BienvenidaPopup()
    {
        InitializeComponent();
        Opened += OnPopupOpened;
    }

    private async void OnPopupOpened(object? sender, EventArgs e)
    {
        // Pequeña demora para que la animación del popup termine
        await Task.Delay(300);
        NombreEntry.Focus();
    }

    private void OnNombreCompleted(object sender, EventArgs e)
    {
        OnComenzarClicked(sender, e);
    }

    private void OnComenzarClicked(object sender, EventArgs e)
    {
        var nombre = NombreEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(nombre))
        {
            NombreEntry.Placeholder = "Por favor, escribe tu nombre...";
            return;
        }

        Resultado.SetResult(nombre);
        Close();
    }
}
