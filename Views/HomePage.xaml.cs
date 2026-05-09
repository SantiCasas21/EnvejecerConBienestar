using EnvejecerConBienestar.ViewModels;

namespace EnvejecerConBienestar.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private readonly HomeViewModel _viewModel;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadHabitosAsync();
    }

    private async void OnVerMedicamentosClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//medicamentos");
    }

    private async void OnVerContactosClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//contactos");
    }
}