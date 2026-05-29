using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvejecerConBienestar.Models;
using EnvejecerConBienestar.Services;

namespace EnvejecerConBienestar.ViewModels;

public partial class PerfilViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private PerfilUsuario _perfil = new();

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _tienePerfil;

    [ObservableProperty]
    private string _mensajeEstado = string.Empty;

    public PerfilViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [RelayCommand]
    public async Task LoadPerfilAsync()
    {
        IsBusy = true;
        var perfil = await _databaseService.GetPerfilUsuarioAsync();
        if (perfil != null)
        {
            Perfil = perfil;
            TienePerfil = true;
        }
        else
        {
            Perfil = new PerfilUsuario();
            TienePerfil = false;
        }
        IsBusy = false;
    }

    [RelayCommand]
    private void ToggleEdit()
    {
        IsEditing = !IsEditing;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Perfil.Nombre))
        {
            await Shell.Current.DisplayAlert("Atencion", "El nombre es obligatorio.", "OK");
            return;
        }

        IsBusy = true;
        await _databaseService.SavePerfilUsuarioAsync(Perfil);

        Preferences.Set("nombre_usuario", Perfil.Nombre);
        MensajeEstado = "Perfil guardado correctamente";
        IsEditing = false;
        TienePerfil = true;
        IsBusy = false;
        
        await Shell.Current.DisplayAlert("✅ Exito", "Tu perfil se ha actualizado correctamente.", "Genial");
    }
}
