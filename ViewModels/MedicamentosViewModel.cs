using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvejecerConBienestar.Models;
using EnvejecerConBienestar.Services;
using CommunityToolkit.Maui.Views;

namespace EnvejecerConBienestar.ViewModels;

public partial class MedicamentosViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly AlarmService _alarmService;

    [ObservableProperty]
    private ObservableCollection<Medicamento> _medicamentos = new();

    [ObservableProperty]
    private ObservableCollection<Medicamento> _sugerencias = new();

    [ObservableProperty]
    private bool _isBusy;

    public MedicamentosViewModel(DatabaseService databaseService, AlarmService alarmService)
    {
        _databaseService = databaseService;
        _alarmService = alarmService;
        LoadSugerencias();
    }

    private void LoadSugerencias()
    {
        Sugerencias = new ObservableCollection<Medicamento>
        {
            new() { Nombre = "Acetaminofén", Icono = "⚪", Miligramos = "500", Frecuencia = 6, Notas = "Para el dolor y la fiebre" },
            new() { Nombre = "Ibuprofeno", Icono = "💊", Miligramos = "400", Frecuencia = 8, Notas = "Antiinflamatorio" },
            new() { Nombre = "Vitamina C", Icono = "🍊", Miligramos = "500", Frecuencia = 24, Notas = "Suplemento diario" },
            new() { Nombre = "Losartán", Icono = "💙", Miligramos = "50", Frecuencia = 24, Notas = "Presión arterial" },
            new() { Nombre = "Metformina", Icono = "🤍", Miligramos = "850", Frecuencia = 12, Notas = "Control de azúcar" }
        };
    }

    [RelayCommand]
    public async Task LoadMedicamentosAsync()
    {
        IsBusy = true;
        var list = await _databaseService.GetMedicamentosAsync();
        
        if (!list.Any())
        {
            var demo = new List<Medicamento>
            {
                new() { Nombre = "Metformina", Miligramos = "500", Frecuencia = 12, HoraAlarma = new TimeSpan(8,0,0), Icono = "🤍", Notas = "Tomar con el desayuno" },
                new() { Nombre = "Losartán", Miligramos = "50", Frecuencia = 24, HoraAlarma = new TimeSpan(20,0,0), Icono = "💙", Notas = "Tomar antes de dormir" }
            };
            foreach (var m in demo) await _databaseService.SaveMedicamentoAsync(m);
            list = await _databaseService.GetMedicamentosAsync();
        }

        // Ordenar: No tomados primero, luego tomados
        var sortedList = list.OrderBy(m => m.EstaTomado).ToList();

        Medicamentos.Clear();
        foreach (var m in sortedList) Medicamentos.Add(m);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task AddMedicamentoAsync()
    {
        var popup = new Views.AddMedicamentoPopup();
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        
        var resultado = await popup.PopupResult.Task;
        
        if (resultado != null)
        {
            await _databaseService.SaveMedicamentoAsync(resultado);
            await _alarmService.ProgramarAlarma(resultado);
            await LoadMedicamentosAsync();
        }
    }

    [RelayCommand]
    private async Task ToggleTomadoAsync(Medicamento medicamento)
    {
        if (medicamento == null) return;
        medicamento.EstaTomado = !medicamento.EstaTomado;
        await _databaseService.SaveMedicamentoAsync(medicamento);

        // Reordenar en el lugar: remover de posición actual y reinsertar en la posición correcta
        var index = Medicamentos.IndexOf(medicamento);
        if (index < 0) return;

        Medicamentos.RemoveAt(index);

        // Si está tomado, va al final (después de todos los no tomados)
        // Si no está tomado, va al principio (antes de todos los tomados)
        if (medicamento.EstaTomado)
        {
            Medicamentos.Add(medicamento);
        }
        else
        {
            var insertIndex = 0;
            while (insertIndex < Medicamentos.Count && !Medicamentos[insertIndex].EstaTomado)
                insertIndex++;
            Medicamentos.Insert(insertIndex, medicamento);
        }
    }

    [RelayCommand]
    private async Task AddSugerenciaAsync(Medicamento sugerencia)
    {
        if (sugerencia == null) return;
        
        bool confirm = await Shell.Current.DisplayAlert("Agregar Medicina", $"¿Desea agregar {sugerencia.Nombre} a su lista diaria?", "Sí, agregar", "Cancelar");
        if (confirm)
        {
            var nuevo = new Medicamento
            {
                Nombre = sugerencia.Nombre,
                Icono = sugerencia.Icono,
                Miligramos = sugerencia.Miligramos,
                Frecuencia = sugerencia.Frecuencia,
                Notas = sugerencia.Notas,
                HoraAlarma = DateTime.Now.TimeOfDay
            };
            await _databaseService.SaveMedicamentoAsync(nuevo);
            await _alarmService.ProgramarAlarma(nuevo);
            
            await Shell.Current.DisplayAlert("Éxito", $"{sugerencia.Nombre} ha sido agregado.", "OK");
            await LoadMedicamentosAsync();
        }
    }

    [RelayCommand]
    private async Task GoToDetailAsync(Medicamento medicamento)
    {
        if (medicamento == null) return;
        await Shell.Current.GoToAsync($"{nameof(Views.MedicamentoDetailPage)}?Id={medicamento.Id}");
    }
}
