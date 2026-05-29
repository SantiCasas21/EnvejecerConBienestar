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
    private readonly ReportService _reportService;

    [ObservableProperty]
    private ObservableCollection<Medicamento> _medicamentos = new();

    [ObservableProperty]
    private ObservableCollection<Medicamento> _sugerencias = new();

    [ObservableProperty]
    private bool _isBusy;

    public MedicamentosViewModel(DatabaseService databaseService, AlarmService alarmService, ReportService reportService)
    {
        _databaseService = databaseService;
        _alarmService = alarmService;
        _reportService = reportService;
        LoadSugerencias();
    }

    private void LoadSugerencias()
    {
        Sugerencias = new ObservableCollection<Medicamento>
        {
            new() { Nombre = "Acetaminofen", Icono = "\uf4a4", ColorIcono = "#0D9488", Miligramos = "500", Frecuencia = 6, Notas = "Para el dolor y la fiebre" },
            new() { Nombre = "Ibuprofeno", Icono = "\uf484", ColorIcono = "#818CF8", Miligramos = "400", Frecuencia = 8, Notas = "Antiinflamatorio" },
            new() { Nombre = "Vitamina C", Icono = "\uf004", ColorIcono = "#22C55E", Miligramos = "500", Frecuencia = 24, Notas = "Suplemento diario" },
            new() { Nombre = "Losartan", Icono = "\uf487", ColorIcono = "#0D9488", Miligramos = "50", Frecuencia = 24, Notas = "Presion arterial" },
            new() { Nombre = "Metformina", Icono = "\uf484", ColorIcono = "#64748B", Miligramos = "850", Frecuencia = 12, Notas = "Control de azucar" }
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
                new() { Nombre = "Metformina", Miligramos = "500", Frecuencia = 12, HoraAlarma = new TimeSpan(8,0,0), Icono = "\uf484", ColorIcono = "#64748B", Notas = "Tomar con el desayuno", CantidadRestante = 30 },
                new() { Nombre = "Losartan", Miligramos = "50", Frecuencia = 24, HoraAlarma = new TimeSpan(20,0,0), Icono = "\uf487", ColorIcono = "#0D9488", Notas = "Tomar antes de dormir", CantidadRestante = 15 }
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

        if (medicamento.EstaTomado && medicamento.CantidadRestante > 0)
        {
            medicamento.CantidadRestante--;

            if (medicamento.AlertaInventario)
            {
                await Shell.Current.DisplayAlert(
                    "⚠️ Inventario Bajo",
                    $"¡Quedan solo {medicamento.CantidadRestante} pastillas de {medicamento.Nombre}!",
                    "Entendido");
            }
        }

        await _databaseService.SaveMedicamentoAsync(medicamento);

        var index = Medicamentos.IndexOf(medicamento);
        if (index < 0) return;

        Medicamentos.RemoveAt(index);

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

    [RelayCommand]
    private async Task GenerateReportAsync()
    {
        IsBusy = true;
        await _reportService.CompartirReporteAsync();
        IsBusy = false;
    }
}
