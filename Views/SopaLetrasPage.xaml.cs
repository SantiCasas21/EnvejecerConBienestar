namespace EnvejecerConBienestar.Views;

public partial class SopaLetrasPage : ContentPage
{
    // ═══════════════════════════════════════════════════════════════════════
    //   MODELO DE PALABRA — posición y orientación en el tablero
    // ═══════════════════════════════════════════════════════════════════════
    private sealed record PalabraInfo(
        string Texto,
        int FilaInicio,
        int ColInicio,
        bool EsHorizontal
    );

    // ═══════════════════════════════════════════════════════════════════════
    //   TABLERO FIJO 8×8 — palabras de salud ocultas
    //
    //       0    1    2    3    4    5    6    7
    //  0:   A    G    U    A    P    L    B    C    ← AGUA (horizontal)
    //  1:   T    E    H    K    N    W    S    A         S (SALUD vertical col6)
    //  2:   M    U    S    I    C    A    A    M    ← MUSICA (horizontal)
    //  3:   X    J    B    Z    Q    Y    L    I         A
    //  4:   V    F    R    U    T    A    U    N    ← FRUTA (horizontal col1-5)
    //  5:   K    H    P    S    W    E    D    A         D
    //  6:   D    O    R    M    I    R    X    R    ← DORMIR (horizontal)
    //  7:   N    B    Y    J    Z    Q    H    K
    //                                    ↑    ↑
    //                               SALUD   CAMINAR  (verticales)
    // ═══════════════════════════════════════════════════════════════════════
    private static readonly char[,] TABLERO = new char[8, 8]
    {
        { 'A','G','U','A','P','L','B','C' },
        { 'T','E','H','K','N','W','S','A' },
        { 'M','U','S','I','C','A','A','M' },
        { 'X','J','B','Z','Q','Y','L','I' },
        { 'V','F','R','U','T','A','U','N' },
        { 'K','H','P','S','W','E','D','A' },
        { 'D','O','R','M','I','R','X','R' },
        { 'N','B','Y','J','Z','Q','H','K' },
    };

    private static readonly PalabraInfo[] PALABRAS = new[]
    {
        new PalabraInfo("AGUA",    0, 0, true ),   // horizontal fila 0, cols 0-3
        new PalabraInfo("MUSICA",  2, 0, true ),   // horizontal fila 2, cols 0-5
        new PalabraInfo("FRUTA",   4, 1, true ),   // horizontal fila 4, cols 1-5
        new PalabraInfo("DORMIR",  6, 0, true ),   // horizontal fila 6, cols 0-5
        new PalabraInfo("CAMINAR", 0, 7, false),   // vertical   col 7, filas 0-6
        new PalabraInfo("SALUD",   1, 6, false),   // vertical   col 6, filas 1-5
    };

    private const int FILAS = 8;
    private const int COLS = 8;

    // ─── Estado del juego ─────────────────────────────────────────────────
    private readonly Frame[,] _celdas = new Frame[FILAS, COLS];
    private readonly Label[,] _letras = new Label[FILAS, COLS];
    private readonly Dictionary<string, (Frame Chip, Label Lbl)> _chips = new();
    private readonly HashSet<string> _encontradas = new();

    private (int Fila, int Col)? _primeraTocada = null;
    private int _segundos = 0;
    private IDispatcherTimer? _timer;

    // ─── Colores (paleta de la app) ───────────────────────────────────────
    private static readonly Color ColNormal = Colors.White;
    private static readonly Color ColBordeNormal = Color.FromArgb("#E5E7EB");
    private static readonly Color ColSel = Color.FromArgb("#FEF3C7");
    private static readonly Color ColBordeSel = Color.FromArgb("#F97316");
    private static readonly Color ColEnc = Color.FromArgb("#D1FAE5");
    private static readonly Color ColBordeEnc = Color.FromArgb("#6EE7B7");
    private static readonly Color ColTextoNormal = Color.FromArgb("#1F2937");
    private static readonly Color ColTextoEnc = Color.FromArgb("#065F46");

    // ═══════════════════════════════════════════════════════════════════════
    public SopaLetrasPage()
    {
        InitializeComponent();
        CrearChipsPalabras();
        ConstruirTablero();
        IniciarTimer();
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   CHIPS VISUALES DE PALABRAS (lista de lo que hay que encontrar)
    // ═══════════════════════════════════════════════════════════════════════
    private void CrearChipsPalabras()
    {
        ContenedorPalabras.Children.Clear();
        _chips.Clear();

        foreach (var pw in PALABRAS)
        {
            var lbl = new Label
            {
                Text = pw.Texto,
                FontFamily = "NunitoSemiBold",
                FontSize = 13,
                TextColor = Color.FromArgb("#0D9488"),
            };
            var chip = new Frame
            {
                BackgroundColor = Color.FromArgb("#CCFBF1"),
                CornerRadius = 10,
                Padding = new Thickness(10, 5),
                HasShadow = false,
                Margin = new Thickness(0, 0, 6, 6),
                Content = lbl,
            };
            _chips[pw.Texto] = (chip, lbl);
            ContenedorPalabras.Add(chip);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   CONSTRUCCIÓN VISUAL DEL TABLERO
    // ═══════════════════════════════════════════════════════════════════════
    private void ConstruirTablero()
    {
        GridTablero.Children.Clear();
        GridTablero.RowDefinitions.Clear();
        GridTablero.ColumnDefinitions.Clear();

        for (int c = 0; c < COLS; c++)
            GridTablero.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        for (int f = 0; f < FILAS; f++)
            GridTablero.RowDefinitions.Add(new RowDefinition(GridLength.Star));

        for (int f = 0; f < FILAS; f++)
        {
            for (int c = 0; c < COLS; c++)
            {
                int fila = f, col = c;   // captura para el closure del evento

                var lbl = new Label
                {
                    Text = TABLERO[f, c].ToString(),
                    FontFamily = "NunitoBold",
                    FontSize = 15,
                    TextColor = ColTextoNormal,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                };

                var celda = new Frame
                {
                    BackgroundColor = ColNormal,
                    BorderColor = ColBordeNormal,
                    CornerRadius = 6,
                    Padding = 0,
                    HasShadow = false,
                    Content = lbl,
                };

                var tap = new TapGestureRecognizer();
                tap.Tapped += (_, _) => OnCeldaTocada(fila, col);
                celda.GestureRecognizers.Add(tap);

                Grid.SetRow(celda, f);
                Grid.SetColumn(celda, c);
                GridTablero.Children.Add(celda);

                _celdas[f, c] = celda;
                _letras[f, c] = lbl;
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   LÓGICA PRINCIPAL — manejo de toques sobre las celdas
    // ═══════════════════════════════════════════════════════════════════════
    private void OnCeldaTocada(int fila, int col)
    {
        // Ignorar celdas que ya pertenecen a una palabra encontrada
        if (EsCeldaEncontrada(fila, col)) return;

        if (_primeraTocada == null)
        {
            // ─ Primer toque: marcar como seleccionada (naranja) ──────────
            _primeraTocada = (fila, col);
            EstiloCelda(fila, col, "seleccionada");
        }
        else
        {
            var (f1, c1) = _primeraTocada.Value;

            // Si toca la misma celda → deseleccionar
            if (f1 == fila && c1 == col)
            {
                EstiloCelda(f1, c1, "normal");
                _primeraTocada = null;
                return;
            }

            // Sólo se aceptan líneas horizontales o verticales (no diagonales)
            if (f1 != fila && c1 != col)
            {
                // Diagonal inválida → convertir segunda celda en primera selección
                EstiloCelda(f1, c1, "normal");
                _primeraTocada = (fila, col);
                EstiloCelda(fila, col, "seleccionada");
                return;
            }

            // ─ Segundo toque válido: leer la palabra trazada ─────────────
            string leida = LeerPalabra(f1, c1, fila, col);
            string leidaInv = new string(leida.Reverse().ToArray());

            // Buscar coincidencia en la lista (ignorar ya encontradas)
            var match = PALABRAS.FirstOrDefault(p =>
                !_encontradas.Contains(p.Texto) &&
                (p.Texto == leida || p.Texto == leidaInv));

            // Quitar highlight de la primera celda siempre
            EstiloCelda(f1, c1, "normal");
            _primeraTocada = null;

            if (match != null)
                MarcarEncontrada(match);
            // Si no hay coincidencia, simplemente se deselecciona sin penalización
        }
    }

    // Lee las letras entre dos celdas en línea recta
    private string LeerPalabra(int f1, int c1, int f2, int c2)
    {
        var sb = new System.Text.StringBuilder();
        if (f1 == f2)  // horizontal
        {
            for (int c = Math.Min(c1, c2); c <= Math.Max(c1, c2); c++)
                sb.Append(TABLERO[f1, c]);
        }
        else           // vertical
        {
            for (int f = Math.Min(f1, f2); f <= Math.Max(f1, f2); f++)
                sb.Append(TABLERO[f, c1]);
        }
        return sb.ToString();
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   MARCAR PALABRA COMO ENCONTRADA
    // ═══════════════════════════════════════════════════════════════════════
    private void MarcarEncontrada(PalabraInfo pw)
    {
        _encontradas.Add(pw.Texto);

        // Pintar verde todas las celdas de la palabra
        for (int i = 0; i < pw.Texto.Length; i++)
        {
            int f = pw.EsHorizontal ? pw.FilaInicio : pw.FilaInicio + i;
            int c = pw.EsHorizontal ? pw.ColInicio + i : pw.ColInicio;
            EstiloCelda(f, c, "encontrada");
        }

        // Tachar el chip en la lista
        if (_chips.TryGetValue(pw.Texto, out var chip))
        {
            chip.Lbl.TextDecorations = TextDecorations.Strikethrough;
            chip.Lbl.TextColor = Color.FromArgb("#9CA3AF");
            chip.Chip.BackgroundColor = Color.FromArgb("#D1FAE5");
        }

        // Actualizar contador
        LblPalabras.Text = $"{_encontradas.Count}/{PALABRAS.Length}";

        if (_encontradas.Count == PALABRAS.Length)
            MostrarVictoria();
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   UTILIDADES VISUALES
    // ═══════════════════════════════════════════════════════════════════════
    private void EstiloCelda(int f, int c, string estado)
    {
        switch (estado)
        {
            case "seleccionada":
                _celdas[f, c].BackgroundColor = ColSel;
                _celdas[f, c].BorderColor = ColBordeSel;
                break;
            case "encontrada":
                _celdas[f, c].BackgroundColor = ColEnc;
                _celdas[f, c].BorderColor = ColBordeEnc;
                _letras[f, c].TextColor = ColTextoEnc;
                break;
            default:
                _celdas[f, c].BackgroundColor = ColNormal;
                _celdas[f, c].BorderColor = ColBordeNormal;
                _letras[f, c].TextColor = ColTextoNormal;
                break;
        }
    }

    private bool EsCeldaEncontrada(int fila, int col)
    {
        foreach (var pw in PALABRAS)
        {
            if (!_encontradas.Contains(pw.Texto)) continue;
            for (int i = 0; i < pw.Texto.Length; i++)
            {
                int f = pw.EsHorizontal ? pw.FilaInicio : pw.FilaInicio + i;
                int c = pw.EsHorizontal ? pw.ColInicio + i : pw.ColInicio;
                if (f == fila && c == col) return true;
            }
        }
        return false;
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   VICTORIA
    // ═══════════════════════════════════════════════════════════════════════
    private void MostrarVictoria()
    {
        _timer?.Stop();
        LblResultado.Text = $"Terminaste en {_segundos} segundos 🌟";
        BannerVictoria.IsVisible = true;
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   TIMER
    // ═══════════════════════════════════════════════════════════════════════
    private void IniciarTimer()
    {
        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) => { _segundos++; LblTiempo.Text = $"{_segundos}s"; };
        _timer.Start();
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   NUEVA PARTIDA
    // ═══════════════════════════════════════════════════════════════════════
    private void OnNuevaPartida(object sender, EventArgs e)
    {
        _timer?.Stop();
        _encontradas.Clear();
        _primeraTocada = null;
        _segundos = 0;
        LblTiempo.Text = "0s";
        LblPalabras.Text = $"0/{PALABRAS.Length}";
        BannerVictoria.IsVisible = false;

        for (int f = 0; f < FILAS; f++)
            for (int c = 0; c < COLS; c++)
                EstiloCelda(f, c, "normal");

        foreach (var pw in PALABRAS)
        {
            if (!_chips.TryGetValue(pw.Texto, out var chip)) continue;
            chip.Lbl.TextDecorations = TextDecorations.None;
            chip.Lbl.TextColor = Color.FromArgb("#0D9488");
            chip.Chip.BackgroundColor = Color.FromArgb("#CCFBF1");
        }

        IniciarTimer();
    }

    private async void OnVolverTocado(object sender, TappedEventArgs e)
    {
        _timer?.Stop();
        await Navigation.PopAsync();
    }
}
