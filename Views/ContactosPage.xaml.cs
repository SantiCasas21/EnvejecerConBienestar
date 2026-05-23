using EnvejecerConBienestar.Models;
using EnvejecerConBienestar.ViewModels;

namespace EnvejecerConBienestar.Views;

public partial class ContactosPage : ContentPage
{
    private readonly ContactosViewModel _viewModel;

    public ContactosPage(ContactosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

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

    private void OnLlamarClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Contacto contacto)
            _viewModel.LlamarCommand.Execute(contacto);
    }
}
