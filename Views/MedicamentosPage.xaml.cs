using EnvejecerConBienestar.Models;
using EnvejecerConBienestar.ViewModels;

namespace EnvejecerConBienestar.Views;

public partial class MedicamentosPage : ContentPage
{
    public MedicamentosPage(MedicamentosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private readonly MedicamentosViewModel _viewModel;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadMedicamentosAsync();
    }

    private void OnMedicamentoTapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Medicamento medicamento)
            _viewModel.GoToDetailCommand.Execute(medicamento);
    }

    private void OnAddSugerenciaClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Medicamento sugerencia)
            _viewModel.AddSugerenciaCommand.Execute(sugerencia);
    }

    private void OnToggleTomadoClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Medicamento medicamento)
            _viewModel.ToggleTomadoCommand.Execute(medicamento);
    }
}
