using EnvejecerConBienestar.ViewModels;

namespace EnvejecerConBienestar.Views;

public partial class MedicamentoDetailPage : ContentPage
{
    public MedicamentoDetailPage(MedicamentoDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
