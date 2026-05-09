using EnvejecerConBienestar.ViewModels;

namespace EnvejecerConBienestar.Views;

public partial class ContactoDetailPage : ContentPage
{
    public ContactoDetailPage(ContactoDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
