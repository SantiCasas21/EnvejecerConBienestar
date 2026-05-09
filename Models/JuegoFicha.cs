using CommunityToolkit.Mvvm.ComponentModel;

namespace EnvejecerConBienestar.Models;

public partial class JuegoFicha : ObservableObject
{
    public int Id { get; set; }
    public int Valor { get; set; }
    
    [ObservableProperty]
    private bool _estaVolteada;
    
    [ObservableProperty]
    private bool _estaEmparejada;

    [ObservableProperty]
    private string _colorFondo = "#E2E8F0";
}
