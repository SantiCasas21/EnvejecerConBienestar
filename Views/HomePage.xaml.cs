using EnvejecerConBienestar.Models;
using EnvejecerConBienestar.ViewModels;

namespace EnvejecerConBienestar.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.ActualizarSaludo();
        await _viewModel.LoadDataAsync();
        await AnimarEntradaAsync();
    }

    private async Task AnimarEntradaAsync()
    {
        var elementos = this.GetVisualTreeDescendants()
            .OfType<Frame>()
            .Where(f => f.StyleId != "static")
            .ToList();

        foreach (var elemento in elementos)
        {
            elemento.Opacity = 0;
            elemento.TranslationY = 30;
        }

        foreach (var elemento in elementos)
        {
            await Task.WhenAll(
                elemento.FadeTo(1, 400, Easing.CubicOut),
                elemento.TranslateTo(0, 0, 400, Easing.CubicOut));
            await Task.Delay(50);
        }
    }

    // ── Metas ──

    private void OnIncrementarClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Meta meta)
            _viewModel.IncrementarProgresoCommand.Execute(meta);
    }

    private async void OnAddMetaClicked(object sender, EventArgs e)
        => await _viewModel.AddMetaCommand.ExecuteAsync(null);

    // ── Medicamentos ──

    private void OnMarcarProximaTomada(object sender, EventArgs e)
    {
        if (_viewModel.ProximaMedicina is not null)
            _viewModel.MarcarMedicamentoTomadoCommand.Execute(_viewModel.ProximaMedicina);
    }

    private void OnMarcarPendienteTomada(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Medicamento medicamento)
            _viewModel.MarcarMedicamentoTomadoCommand.Execute(medicamento);
    }
}
