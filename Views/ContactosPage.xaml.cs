using EnvejecerConBienestar.Models;
using EnvejecerConBienestar.ViewModels;

namespace EnvejecerConBienestar.Views;

public partial class ContactosPage : ContentPage
{
    public ContactosPage(ContactosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private readonly ContactosViewModel _viewModel;

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataAsync();
    }

    private void OnContactoTapped(object sender, TappedEventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Contacto contacto)
            _viewModel.GoToDetailCommand.Execute(contacto);
    }
}
