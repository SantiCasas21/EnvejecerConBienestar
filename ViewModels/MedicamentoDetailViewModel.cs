using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvejecerConBienestar.Models;
using EnvejecerConBienestar.Services;

namespace EnvejecerConBienestar.ViewModels;

[QueryProperty(nameof(MedicamentoId), "Id")]
public partial class MedicamentoDetailViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly AlarmService _alarmService;

    [ObservableProperty]
    private int _medicamentoId;

    [ObservableProperty]
    private Medicamento _medicamento = new();

    [ObservableProperty]
    private bool _isEditing;

    public MedicamentoDetailViewModel(DatabaseService databaseService, AlarmService alarmService)
    {
        _databaseService = databaseService;
        _alarmService = alarmService;
    }

    async partial void OnMedicamentoIdChanged(int value)
    {
        if (value > 0)
        {
            Medicamento = await _databaseService.GetMedicamentoAsync(value);
            IsEditing = false;
        }
    }

    [RelayCommand]
    private void ToggleEdit() => IsEditing = !IsEditing;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Medicamento.Nombre)) 
        {
            await Shell.Current.DisplayAlert("Error", "El nombre es obligatorio.", "OK");
            return;
        }

        await _databaseService.SaveMedicamentoAsync(Medicamento);
        await _alarmService.ProgramarAlarma(Medicamento);
        IsEditing = false;
        await Shell.Current.DisplayAlert("Guardado", "Información actualizada correctamente.", "OK");
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert("Borrar", $"¿Desea eliminar {Medicamento.Nombre} de la lista?", "Sí, eliminar", "No");
        if (confirm)
        {
            await _databaseService.DeleteItemAsync(Medicamento);
            await Shell.Current.GoToAsync("..");
        }
    }

    [RelayCommand]
    private async Task BackAsync() => await Shell.Current.GoToAsync("..");
}
