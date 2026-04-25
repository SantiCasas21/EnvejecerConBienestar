using EnvejecerConBienestar.ViewModels;

namespace EnvejecerConBienestar.Views;

public partial class HomePage : ContentPage
{
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnVerMedicamentosClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//medicamentos");
    }
}