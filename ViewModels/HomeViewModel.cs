using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvejecerConBienestar.Models;
using EnvejecerConBienestar.Services;
using CommunityToolkit.Maui.Views;

namespace EnvejecerConBienestar.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly DatabaseService _databaseService;
    private readonly ContactService _contactService;

    // ── Saludo ──
    [ObservableProperty]
    private string _saludo;

    [ObservableProperty]
    private string _nombreUsuario = string.Empty;

    [ObservableProperty]
    private string _emojiSaludo = "👋";

    [ObservableProperty]
    private bool _isBusy;

    // ── Metas (Logros del Día) ──
    [ObservableProperty]
    private ObservableCollection<Meta> _metas = new();

    [ObservableProperty]
    private int _metasCompletadasHoy;

    [ObservableProperty]
    private int _totalMetasHoy;

    [ObservableProperty]
    private string _resumenDiario = string.Empty;

    [ObservableProperty]
    private double _porcentajeDiario;

    // ── Medicamentos ──
    [ObservableProperty]
    private ObservableCollection<Medicamento> _medicamentos = new();

    [ObservableProperty]
    private Medicamento? _proximaMedicina;

    [ObservableProperty]
    private string _proximaHora = "";

    [ObservableProperty]
    private string _proximaEtiqueta = "";

    [ObservableProperty]
    private ObservableCollection<Medicamento> _medicinasPendientes = new();

    [ObservableProperty]
    private bool _hayMedicamentos;

    [ObservableProperty]
    private bool _hayPendientes;

    [ObservableProperty]
    private bool _hayProxima;

    [ObservableProperty]
    private bool _todasTomadas;

    public HomeViewModel(DatabaseService databaseService, ContactService contactService)
    {
        _databaseService = databaseService;
        _contactService = contactService;

        CargarNombreGuardado();
        ActualizarSaludo();
    }

    // ═══════════════════════════════════════════
    //  SALUDO
    // ═══════════════════════════════════════════

    private void CargarNombreGuardado()
    {
        NombreUsuario = Preferences.Get("nombre_usuario", string.Empty);
    }

    public void ActualizarSaludo()
    {
        int hora = DateTime.Now.Hour;

        if (hora < 12)
        {
            Saludo = "¡Buenos días";
            EmojiSaludo = "☀️";
        }
        else if (hora < 18)
        {
            Saludo = "¡Buenas tardes";
            EmojiSaludo = "🌤️";
        }
        else
        {
            Saludo = "¡Buenas noches";
            EmojiSaludo = "🌙";
        }

        Saludo = string.IsNullOrWhiteSpace(NombreUsuario)
            ? $"{Saludo}!"
            : $"{Saludo}, {NombreUsuario}!";
    }

    public async Task VerificarPrimerInicioAsync()
    {
        if (string.IsNullOrWhiteSpace(NombreUsuario))
        {
            var popup = new Views.BienvenidaPopup();
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);

            var nombre = await popup.Resultado.Task;
            if (!string.IsNullOrWhiteSpace(nombre))
            {
                NombreUsuario = nombre;
                Preferences.Set("nombre_usuario", nombre);
                ActualizarSaludo();
            }
        }
    }

    // ═══════════════════════════════════════════
    //  CARGA PRINCIPAL (se ejecuta en cada OnAppearing)
    // ═══════════════════════════════════════════

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        IsBusy = true;

        await VerificarPrimerInicioAsync();
        await CargarMedicamentosAsync();
        await CargarMetasAsync();

        ActualizarMedicamentosUI();
        ActualizarResumenMetas();

        IsBusy = false;
    }

    // ═══════════════════════════════════════════
    //  MEDICAMENTOS
    // ═══════════════════════════════════════════

    private async Task CargarMedicamentosAsync()
    {
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

        Medicamentos.Clear();
        foreach (var m in list) Medicamentos.Add(m);
    }

    private void ActualizarMedicamentosUI()
    {
        var ahora = DateTime.Now.TimeOfDay;

        HayMedicamentos = Medicamentos.Count > 0;

        // Encontrar la próxima medicina: no tomada, con dosis pendiente hoy, la más cercana
        var pendientes = Medicamentos.Where(m => !m.EstaTomado).ToList();

        ProximaMedicina = pendientes
            .Where(m => m.HorarioDiario.Any(h => h > ahora))
            .OrderBy(m => m.HorarioDiario.Where(h => h > ahora).Min())
            .FirstOrDefault();

        if (ProximaMedicina != null)
        {
            var proxDosis = ProximaMedicina.HorarioDiario
                .Where(h => h > ahora)
                .Min();
            ProximaHora = proxDosis.ToString(@"hh\:mm");

            var diff = proxDosis - ahora;
            if (diff.TotalMinutes <= 0)
                ProximaEtiqueta = "AHORA";
            else if (diff.TotalMinutes < 60)
                ProximaEtiqueta = $"en {diff.Minutes} min";
            else
                ProximaEtiqueta = $"a las {ProximaHora}";

            // Las demás pendientes (excluyendo la próxima)
            MedicinasPendientes = new ObservableCollection<Medicamento>(
                pendientes.Where(m => m.Id != ProximaMedicina.Id));
        }
        else
        {
            ProximaHora = "";
            ProximaEtiqueta = "";
            MedicinasPendientes = new ObservableCollection<Medicamento>(pendientes);
        }

        HayProxima = ProximaMedicina != null;
        HayPendientes = ProximaMedicina != null || MedicinasPendientes.Count > 0;
        TodasTomadas = HayMedicamentos && !HayPendientes;
    }

    [RelayCommand]
    private async Task MarcarMedicamentoTomadoAsync(Medicamento medicamento)
    {
        if (medicamento == null || medicamento.EstaTomado) return;

        medicamento.EstaTomado = true;
        await _databaseService.SaveMedicamentoAsync(medicamento);

        ActualizarMedicamentosUI();

        await Shell.Current.DisplayAlert(
            "✅ Medicamento Tomado",
            $"Has registrado {medicamento.Icono} {medicamento.Nombre}. ¡Muy responsable!",
            "OK");
    }

    // ═══════════════════════════════════════════
    //  METAS (Logros del Día)
    // ═══════════════════════════════════════════

    private async Task CargarMetasAsync()
    {
        await EvaluarMetasVencidasAsync();
        var activas = await _databaseService.GetMetasActivasAsync();

        if (!activas.Any())
        {
            await InicializarMetasDemo();
            activas = await _databaseService.GetMetasActivasAsync();
        }

        Metas.Clear();
        foreach (var m in activas.OrderBy(m => m.Completada)) Metas.Add(m);
    }

    private async Task InicializarMetasDemo()
    {
        var ahora = DateTime.Now;
        var finDia = ahora.Date.AddDays(1).AddSeconds(-1);

        var demo = new List<Meta>
        {
            new() { Nombre = "Agua", Icono = "💧", Objetivo = 8, Progreso = 0, Unidad = "vasos", Frecuencia = "Diaria", FechaInicio = ahora, FechaFin = finDia },
            new() { Nombre = "Caminata", Icono = "🚶", Objetivo = 30, Progreso = 0, Unidad = "minutos", Frecuencia = "Diaria", FechaInicio = ahora, FechaFin = finDia },
            new() { Nombre = "Ejercicio", Icono = "💪", Objetivo = 1, Progreso = 0, Unidad = "sesión", Frecuencia = "Diaria", FechaInicio = ahora, FechaFin = finDia }
        };

        foreach (var m in demo) await _databaseService.SaveMetaAsync(m);
    }

    private async Task EvaluarMetasVencidasAsync()
    {
        var pendientes = await _databaseService.GetMetasPendientesAsync();
        if (!pendientes.Any()) return;

        var mensajes = new List<string>();
        foreach (var m in pendientes)
        {
            m.Completada = true;
            await _databaseService.SaveMetaAsync(m);
            mensajes.Add($"• {m.Icono} {m.Nombre}: {m.Recomendacion}");
        }

        if (mensajes.Any())
        {
            var titulo = pendientes.Count == 1
                ? $"Tu meta de {pendientes[0].Nombre} no se completó"
                : "Algunas metas no se completaron";

            var cuerpo = "No te desanimes. Aquí tienes consejos para lograrlo:\n\n" + string.Join("\n", mensajes);

            await Shell.Current.DisplayAlert(titulo, cuerpo, "¡Lo intentaré de nuevo!");
        }
    }

    [RelayCommand]
    private async Task IncrementarProgresoAsync(Meta meta)
    {
        if (meta == null || meta.Completada) return;

        meta.Progreso++;

        if (meta.Progreso >= meta.Objetivo)
        {
            meta.Progreso = meta.Objetivo;
            meta.Completada = true;
            await _databaseService.SaveMetaAsync(meta);

            RefrescarMetaEnLista(meta);
            await MostrarCelebracionAsync(meta);
        }
        else
        {
            await _databaseService.SaveMetaAsync(meta);
            RefrescarMetaEnLista(meta);
            ActualizarResumenMetas();
        }
    }

    private void RefrescarMetaEnLista(Meta meta)
    {
        var index = Metas.IndexOf(meta);
        if (index < 0) return;
        Metas.RemoveAt(index);
        Metas.Insert(index, meta);
    }

    private async Task MostrarCelebracionAsync(Meta meta)
    {
        ActualizarResumenMetas();
        await Shell.Current.DisplayAlert(
            "🎉 ¡Meta Cumplida!",
            $"{meta.Icono} Completaste: {meta.Nombre}\n{meta.Progreso} de {meta.Objetivo} {meta.Unidad}\n\n{meta.MensajeExito}",
            "¡Qué alegría!");
    }

    private void ActualizarResumenMetas()
    {
        MetasCompletadasHoy = Metas.Count(m => m.Completada);
        TotalMetasHoy = Metas.Count;
        PorcentajeDiario = TotalMetasHoy > 0 ? (double)MetasCompletadasHoy / TotalMetasHoy : 0;
        ResumenDiario = TotalMetasHoy > 0
            ? $"Has completado {MetasCompletadasHoy} de {TotalMetasHoy} metas hoy"
            : "Crea tu primera meta para empezar";
    }

    [RelayCommand]
    private async Task AddMetaAsync()
    {
        var popup = new Views.AddMetaPopup();
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);

        var resultado = await popup.PopupResult.Task;
        if (resultado != null)
        {
            await _databaseService.SaveMetaAsync(resultado);

            if (resultado.FechaFin >= DateTime.Now)
            {
                Metas.Add(resultado);
                ActualizarResumenMetas();
            }
        }
    }

    // ═══════════════════════════════════════════
    //  EMERGENCIA
    // ═══════════════════════════════════════════

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
                "No se encontró un contacto de emergencia. Asígnelo en la sección de Contactos.",
                "Entendido");
        }
    }
}
