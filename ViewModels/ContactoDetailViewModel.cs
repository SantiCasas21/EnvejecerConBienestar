using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvejecerConBienestar.Models;
using EnvejecerConBienestar.Services;

namespace EnvejecerConBienestar.ViewModels;

[QueryProperty(nameof(ContactoId), "Id")]
public partial class ContactoDetailViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly ContactService _contactService;

    [ObservableProperty]
    private int _contactoId;

    [ObservableProperty]
    private Contacto _contacto = new();

    [ObservableProperty]
    private bool _isEditing;

    public ContactoDetailViewModel(DatabaseService databaseService, ContactService contactService)
    {
        _databaseService = databaseService;
        _contactService = contactService;
    }

    async partial void OnContactoIdChanged(int value)
    {
        if (value > 0)
        {
            Contacto = await _databaseService.GetContactoAsync(value);
            IsEditing = false;
        }
        else
        {
            Contacto = new Contacto { Icono = "👤" };
            IsEditing = true;
        }
    }

    [RelayCommand]
    private void ToggleEdit()
    {
        IsEditing = !IsEditing;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Contacto.Nombre))
        {
            await Shell.Current.DisplayAlert("Error", "El nombre es obligatorio.", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Contacto.ColorAvatar))
            Contacto.ColorAvatar = Contacto.GenerarColorPorNombre(Contacto.Nombre);

        await _databaseService.SaveContactoAsync(Contacto);
        IsEditing = false;
        await Shell.Current.DisplayAlert("Guardado", $"{Contacto.Nombre} se actualizó correctamente.", "OK");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Eliminar Contacto",
            $"¿Desea eliminar a {Contacto.Nombre} de su agenda?",
            "Sí, eliminar", "Cancelar");

        if (confirm)
        {
            await _databaseService.DeleteItemAsync(Contacto);
            await Shell.Current.GoToAsync("..");
        }
    }

    [RelayCommand]
    private async Task CallAsync()
    {
        if (string.IsNullOrWhiteSpace(Contacto.Telefono))
        {
            await Shell.Current.DisplayAlert("Sin teléfono", "Este contacto no tiene un número registrado.", "OK");
            return;
        }
        await _contactService.RealizarLlamada(Contacto.Telefono);
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(Contacto.Telefono))
        {
            await Shell.Current.DisplayAlert("Sin teléfono", "Este contacto no tiene un número registrado.", "OK");
            return;
        }
        await _contactService.EnviarMensaje(Contacto.Telefono);
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
