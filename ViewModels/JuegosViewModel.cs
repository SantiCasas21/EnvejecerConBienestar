using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvejecerConBienestar.Models;
using System.Diagnostics;

namespace EnvejecerConBienestar.ViewModels;

public partial class JuegosViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<JuegoFicha> _fichas = new();

    [ObservableProperty]
    private int _intentos;

    [ObservableProperty]
    private string _tiempoTranscurrido = "00:00";

    [ObservableProperty]
    private bool _juegoTerminado;

    private Stopwatch _stopwatch = new();
    private IDispatcherTimer _timer;
    private JuegoFicha? _primeraFicha;
    private bool _estaProcesando;

    public JuegosViewModel()
    {
        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (s, e) => TiempoTranscurrido = _stopwatch.Elapsed.ToString(@"mm\:ss");
        
        IniciarJuego();
    }

    [RelayCommand]
    private void IniciarJuego()
    {
        _stopwatch.Reset();
        _timer.Stop();
        TiempoTranscurrido = "00:00";
        Intentos = 0;
        JuegoTerminado = false;
        _primeraFicha = null;
        _estaProcesando = false;

        var valores = Enumerable.Range(1, 6).Concat(Enumerable.Range(1, 6)).OrderBy(x => Guid.NewGuid()).ToList();
        
        Fichas.Clear();
        for (int i = 0; i < valores.Count; i++)
        {
            Fichas.Add(new JuegoFicha { Id = i, Valor = valores[i] });
        }
    }

    [RelayCommand]
    private async Task VoltearFichaAsync(JuegoFicha ficha)
    {
        if (_estaProcesando || ficha.EstaVolteada || ficha.EstaEmparejada || JuegoTerminado)
            return;

        if (!_stopwatch.IsRunning)
        {
            _stopwatch.Start();
            _timer.Start();
        }

        ficha.EstaVolteada = true;

        if (_primeraFicha == null)
        {
            _primeraFicha = ficha;
        }
        else
        {
            _estaProcesando = true;
            Intentos++;

            if (_primeraFicha.Valor == ficha.Valor)
            {
                _primeraFicha.EstaEmparejada = true;
                ficha.EstaEmparejada = true;
                _primeraFicha = null;
                _estaProcesando = false;

                if (Fichas.All(f => f.EstaEmparejada))
                {
                    _stopwatch.Stop();
                    _timer.Stop();
                    JuegoTerminado = true;
                    await Shell.Current.DisplayAlert("¡Excelente!", $"Has completado el juego en {Intentos} intentos y un tiempo de {TiempoTranscurrido}.", "¡Genial!");
                }
            }
            else
            {
                await Task.Delay(1000);
                _primeraFicha.EstaVolteada = false;
                ficha.EstaVolteada = false;
                _primeraFicha = null;
                _estaProcesando = false;
            }
        }
    }
}
