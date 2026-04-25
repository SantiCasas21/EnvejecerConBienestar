using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvejecerConBienestar.Models;

namespace EnvejecerConBienestar.ViewModels;

public partial class MedicamentosViewModel : ObservableObject
{
    // ---- Lista reactiva de medicamentos ----
    public ObservableCollection<Medicamento> Medicamentos { get; } = new();

    [ObservableProperty]
    private string _fechaSeleccionada;

    [ObservableProperty]
    private int _totalTomados;

    [ObservableProperty]
    private int _totalPendientes;

    public MedicamentosViewModel()
    {
        FechaSeleccionada = DateTime.Now.ToString("dddd d 'de' MMMM",
            new System.Globalization.CultureInfo("es-CO"));

        if (FechaSeleccionada.Length > 0)
            FechaSeleccionada = char.ToUpper(FechaSeleccionada[0]) + FechaSeleccionada[1..];

        CargarDatosEjemplo();
        ActualizarContadores();
    }

    // ---- Datos hardcoded para el prototipo ----

    private void CargarDatosEjemplo()
    {
        var datos = new List<Medicamento>
        {
            new() { Id = 1, Nombre = "Metformina",     Dosis = "500 mg",  Hora = "8:00 a.m.",  Icono = "💊", Tomado = true  },
            new() { Id = 2, Nombre = "Losartán",       Dosis = "50 mg",   Hora = "8:00 a.m.",  Icono = "💙", Tomado = true  },
            new() { Id = 3, Nombre = "Atorvastatina",  Dosis = "20 mg",   Hora = "12:00 p.m.", Icono = "💊", Tomado = false },
            new() { Id = 4, Nombre = "Vitamina D",     Dosis = "1000 UI", Hora = "12:00 p.m.", Icono = "🟡", Tomado = false },
            new() { Id = 5, Nombre = "Omeprazol",      Dosis = "20 mg",   Hora = "6:00 p.m.",  Icono = "💊", Tomado = false },
            new() { Id = 6, Nombre = "Aspirina",       Dosis = "100 mg",  Hora = "8:00 p.m.",  Icono = "🔴", Tomado = false },
        };

        foreach (var m in datos)
            Medicamentos.Add(m);
    }

    private void ActualizarContadores()
    {
        TotalTomados    = Medicamentos.Count(m => m.Tomado);
        TotalPendientes = Medicamentos.Count(m => !m.Tomado);
    }

    // ---- Comando: marcar / desmarcar medicamento ----

    [RelayCommand]
    private async Task ToggleTomado(Medicamento medicamento)
    {
        if (medicamento is null) return;

        medicamento.Tomado = !medicamento.Tomado;

        // Refrescar el item en la colección para que la UI reaccione
        var index = Medicamentos.IndexOf(medicamento);
        if (index >= 0)
        {
            Medicamentos.RemoveAt(index);
            Medicamentos.Insert(index, medicamento);
        }

        ActualizarContadores();

        if (medicamento.Tomado)
        {
            // Feedback positivo breve
            await Shell.Current.DisplayAlert(
                "¡Muy bien! 🎉",
                $"Anotado. Tomó su {medicamento.Nombre} a tiempo.\n¡Siga así!",
                "Gracias");
        }
    }
}
