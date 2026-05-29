using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvejecerConBienestar.Models;
using EnvejecerConBienestar.Services;
using EnvejecerConBienestar.Views;
using CommunityToolkit.Maui.Views;

namespace EnvejecerConBienestar.ViewModels;

public partial class ContactosViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly ContactService _contactService;

    [ObservableProperty]
    private ObservableCollection<Contacto> _emergencias = new();

    [ObservableProperty]
    private ObservableCollection<Contacto> _favoritos = new();

    [ObservableProperty]
    private ObservableCollection<Contacto> _otros = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private int _totalContactos;

    [ObservableProperty]
    private bool _sinContactos;

    public ContactosViewModel(DatabaseService databaseService, ContactService contactService)
    {
        _databaseService = databaseService;
        _contactService = contactService;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        IsBusy = true;
        var all = await _databaseService.GetContactosAsync();

        Emergencias.Clear();
        Favoritos.Clear();
        Otros.Clear();

        foreach (var c in all)
        {
            if (c.EsEmergencia) Emergencias.Add(c);
            else if (c.EsFavorito) Favoritos.Add(c);
            else Otros.Add(c);
        }

        TotalContactos = all.Count;
        SinContactos = all.Count == 0;
        IsBusy = false;
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Contacto contacto)
    {
        if (contacto == null) return;
        await Shell.Current.GoToAsync($"{nameof(ContactoDetailPage)}?Id={contacto.Id}");
    }

    [RelayCommand]
    private async Task LlamarAsync(Contacto contacto)
    {
        if (contacto == null) return;
        await _contactService.RealizarLlamada(contacto.Telefono);
    }

    [RelayCommand]
    private async Task ImportarDesdeAgendaAsync()
    {
        try
        {
            var contactoNativo = await Microsoft.Maui.ApplicationModel.Communication.Contacts.Default.PickContactAsync();
            if (contactoNativo == null) return;

            var nombre = contactoNativo.DisplayName ?? "Sin nombre";
            var telefono = contactoNativo.Phones?.FirstOrDefault()?.PhoneNumber ?? "";

            if (string.IsNullOrWhiteSpace(nombre) && string.IsNullOrWhiteSpace(telefono))
            {
                await Shell.Current.DisplayAlert("Sin datos", "El contacto seleccionado no tiene nombre ni teléfono.", "OK");
                return;
            }

            var nuevo = new Contacto
            {
                Nombre = nombre,
                Telefono = telefono,
                Icono = "",
                ColorAvatar = Contacto.GenerarColorPorNombre(nombre)
            };

            await _databaseService.SaveContactoAsync(nuevo);
            await LoadDataAsync();

            await Shell.Current.DisplayAlert("✅ Contacto Importado",
                $"'{nombre}' se ha agregado a tu agenda de confianza.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo importar el contacto: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task AddNewAsync()
    {
        var popup = new AddContactoPopup();
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);

        var resultado = await popup.PopupResult.Task;
        if (resultado != null)
        {
            if (string.IsNullOrWhiteSpace(resultado.ColorAvatar))
                resultado.ColorAvatar = Contacto.GenerarColorPorNombre(resultado.Nombre);

            await _databaseService.SaveContactoAsync(resultado);
            await LoadDataAsync();
        }
    }
}
