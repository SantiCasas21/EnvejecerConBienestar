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

        await _databaseService.SaveContactoAsync(Contacto);
        IsEditing = false;
        await Shell.Current.DisplayAlert("Éxito", "Contacto guardado correctamente.", "OK");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert("Borrar", $"¿Desea eliminar a {Contacto.Nombre}?", "Sí, eliminar", "No");
        if (confirm)
        {
            await _databaseService.DeleteItemAsync(Contacto);
            await Shell.Current.GoToAsync("..");
        }
    }

    [RelayCommand]
    private async Task CallAsync()
    {
        await _contactService.RealizarLlamada(Contacto.Telefono);
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
