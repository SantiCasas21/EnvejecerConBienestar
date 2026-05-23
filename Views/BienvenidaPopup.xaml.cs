using CommunityToolkit.Maui.Views;

namespace EnvejecerConBienestar.Views;

public partial class BienvenidaPopup : Popup
{
    public TaskCompletionSource<string> Resultado { get; } = new();

    public BienvenidaPopup()
    {
        InitializeComponent();
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
