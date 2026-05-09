using CommunityToolkit.Maui.Views;
using EnvejecerConBienestar.Models;

namespace EnvejecerConBienestar.Views;

public partial class AddMedicamentoPopup : Popup
{
    public TaskCompletionSource<Medicamento?> PopupResult { get; } = new();

    public AddMedicamentoPopup()
    {
        InitializeComponent();
    }

    private void OnSugerenciaClicked(object sender, EventArgs e)
    {
        if (sender is Button btn)
        {
            NombreEntry.Text = btn.Text;
        }
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
            return;
        }

        var med = new Medicamento
        {
            Nombre = NombreEntry.Text,
            Miligramos = MgEntry.Text ?? "",
            Frecuencia = int.TryParse(FrecuenciaEntry.Text, out int f) ? f : 8,
            HoraAlarma = TimePicker.Time,
            FechaInicio = DateTime.Now
        };

        PopupResult.SetResult(med);
        Close();
    }
}
