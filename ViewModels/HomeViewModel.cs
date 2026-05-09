using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvejecerConBienestar.Models;
using EnvejecerConBienestar.Services;

namespace EnvejecerConBienestar.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;

    [ObservableProperty]
    private ObservableCollection<Habito> _habitos = new();

    [ObservableProperty]
    private string _saludo;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _nombreUsuario = "Santiago"; // Estático por ahora

    private readonly ContactService _contactService;

    public HomeViewModel(DatabaseService databaseService, ContactService contactService)
    {
        _databaseService = databaseService;
        _contactService = contactService;
        
        int hora = DateTime.Now.Hour;
        string saludoBase = hora < 12 ? "¡Buenos días" : (hora < 18 ? "¡Buenas tardes" : "¡Buenas noches");
        Saludo = $"{saludoBase}, {NombreUsuario}!";
    }

    [RelayCommand]
    private async Task LlamadaEmergenciaAsync()
    {
        var sos = await _databaseService.GetContactoEmergenciaAsync();
        if (sos != null && !string.IsNullOrWhiteSpace(sos.Telefono))
        {
            await _contactService.RealizarLlamada(sos.Telefono);
        }
        else
        {
            await Shell.Current.DisplayAlert("Configuración SOS", 
                "No se encontró un contacto de emergencia marcado como SOS. Por favor, asigne uno en la sección de Contactos.", 
                "Entendido");
        }
    }

    [RelayCommand]
    public async Task LoadHabitosAsync()
    {
        IsBusy = true;
        var list = await _databaseService.GetHabitosAsync(DateTime.Now);

        if (!list.Any())
        {
            // Inicializar hábitos por defecto para el día si no existen
            var iniciales = new List<Habito>
            {
                new Habito { Tipo = "Agua", Meta = 8, ProgresoActual = 0, Fecha = DateTime.Now.Date },
                new Habito { Tipo = "Caminata", Meta = 30, ProgresoActual = 0, Fecha = DateTime.Now.Date },
                new Habito { Tipo = "Ejercicio", Meta = 1, ProgresoActual = 0, Fecha = DateTime.Now.Date }
            };

            foreach (var h in iniciales)
                await _databaseService.SaveHabitoAsync(h);
            
            list = await _databaseService.GetHabitosAsync(DateTime.Now);
        }

        Habitos.Clear();
        foreach (var h in list) Habitos.Add(h);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task ActualizarProgresoAsync(Habito habito)
    {
        if (habito == null) return;

        string input = await Shell.Current.DisplayPromptAsync("Actualizar", $"¿Cuánto sumaste a {habito.Tipo}?", keyboard: Keyboard.Numeric);
        
        if (int.TryParse(input, out int valor))
        {
            habito.ProgresoActual += valor;
            if (habito.ProgresoActual > habito.Meta && habito.Tipo != "Agua") 
                habito.ProgresoActual = habito.Meta;

            await _databaseService.SaveHabitoAsync(habito);
            
            // Refrescar UI (Truco para forzar actualización en el carrusel)
            var index = Habitos.IndexOf(habito);
            if (index >= 0)
            {
                Habitos.RemoveAt(index);
                Habitos.Insert(index, habito);
            }
        }
    }
}
