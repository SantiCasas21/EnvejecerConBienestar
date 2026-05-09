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

    [ObservableProperty]
    private ObservableCollection<Contacto> _favoritos = new();

    [ObservableProperty]
    private ObservableCollection<Contacto> _emergencias = new();

    [ObservableProperty]
    private ObservableCollection<Contacto> _otros = new();

    [ObservableProperty]
    private bool _isBusy;

    public ContactosViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        IsBusy = true;
        var all = await _databaseService.GetContactosAsync();

        Favoritos.Clear();
        Emergencias.Clear();
        Otros.Clear();

        foreach (var c in all)
        {
            if (c.EsEmergencia) Emergencias.Add(c);
            else if (c.EsFavorito) Favoritos.Add(c);
            else Otros.Add(c);
        }
        IsBusy = false;
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Contacto contacto)
    {
        if (contacto == null) return;
        await Shell.Current.GoToAsync($"{nameof(ContactoDetailPage)}?Id={contacto.Id}");
    }

    [RelayCommand]
    private async Task AddNewAsync()
    {
        var popup = new AddContactoPopup();
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        
        var resultado = await popup.PopupResult.Task;
        if (resultado != null)
        {
            await _databaseService.SaveContactoAsync(resultado);
            await LoadDataAsync();
        }
    }
}
