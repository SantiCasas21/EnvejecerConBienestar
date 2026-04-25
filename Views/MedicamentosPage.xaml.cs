using EnvejecerConBienestar.ViewModels;

namespace EnvejecerConBienestar.Views;

public partial class MedicamentosPage : ContentPage
{
    public MedicamentosPage(MedicamentosViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

// ---- Convertidor: bool → Color de fondo del botón ----
public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool tomado && tomado)
            return Color.FromArgb("#DCFCE7"); // Verde claro si tomado
        return Color.FromArgb("#0D9488");     // Verde oscuro (acción pendiente)
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}

// ---- Convertidor: bool → Color de texto del botón ----
public class BoolToTextColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        if (value is bool tomado && tomado)
            return Color.FromArgb("#16A34A"); // Texto verde oscuro
        return Colors.White;                  // Texto blanco sobre verde
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        => throw new NotImplementedException();
}
