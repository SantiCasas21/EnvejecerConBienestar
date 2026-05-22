namespace EnvejecerConBienestar.Views;

public partial class BuscarParesPage : ContentPage
{
    // ── Clase auxiliar para guardar referencias visuales de cada carta ──────
    // (Evita el RuntimeBinderException que ocurre al usar Tuples con dynamic)
    private sealed class CartaReferencias
    {
        public VerticalStackLayout Dorso { get; }
        public VerticalStackLayout Cara { get; }
        public CartaReferencias(VerticalStackLayout dorso, VerticalStackLayout cara)
        { Dorso = dorso; Cara = cara; }
    }

    // ── Catálogo de cartas temáticas ─────────────────────────────────────────
    private readonly List<(string Emoji, string Etiqueta)> _catalogo = new()
    {
        ("🥦","Nutrición"), ("🚶","Caminar"),   ("💊","Medicamentos"),
        ("😴","Descanso"),  ("🧠","Memoria"),   ("❤️","Corazón"),
        ("🌞","Vitamina D"),("🤝","Acompañar"), ("🧘","Relajación"),
        ("💧","Hidratación"),("📖","Lectura"),  ("🎵","Música"),
        ("🌿","Naturaleza"),("🏋️","Ejercicio"), ("🍎","Frutas"),
        ("😊","Alegría"),
    };

    // ── Estado del juego ─────────────────────────────────────────────────────
    private record CartaEstado(int PairId, string Emoji, string Etiqueta)
    {
        public bool Volteada { get; set; } = false;
        public bool Emparejada { get; set; } = false;
    }

    private List<CartaEstado> _cartas = new();
    private readonly List<Frame> _frames = new();
    private readonly List<int> _esperando = new();
    private bool _bloqueado = false;
    private int _intentos = 0;
    private int _paresEncontrados = 0;
    private int _totalPares = 8;
    private int _segundos = 0;
    private IDispatcherTimer? _timer;

    // ── Colores de la paleta ──────────────────────────────────────────────────
    private static readonly Color ColDorsoFondo = Color.FromArgb("#EDE9FE");
    private static readonly Color ColDorsoTexto = Color.FromArgb("#7C3AED");
    private static readonly Color ColCaraFondo = Color.FromArgb("#FFFFFF");
    private static readonly Color ColMatchFondo = Color.FromArgb("#D1FAE5");
    private static readonly Color ColMatchBorde = Color.FromArgb("#6EE7B7");
    private static readonly Color ColMatchTexto = Color.FromArgb("#065F46");
    private static readonly Color ColBorde = Color.FromArgb("#E5E7EB");

    // ════════════════════════════════════════════════════════════════
    public BuscarParesPage()
    {
        InitializeComponent();
        IniciarJuego();
    }

    // ── Iniciar / reiniciar ───────────────────────────────────────────────────
    private void IniciarJuego()
    {
        _timer?.Stop();
        _intentos = 0; _paresEncontrados = 0; _segundos = 0;
        _bloqueado = false;
        _esperando.Clear();
        _frames.Clear();

        LblIntentos.Text = "0";
        LblPares.Text = $"0/{_totalPares}";
        LblTiempo.Text = "0s";
        BannerVictoria.IsVisible = false;

        _cartas = GenerarCartas(_totalPares);
        ConstruirTablero();

        _timer = Application.Current!.Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(1);
        _timer.Tick += (_, _) => { _segundos++; LblTiempo.Text = $"{_segundos}s"; };
        _timer.Start();
    }

    private List<CartaEstado> GenerarCartas(int total)
    {
        var sel = _catalogo.OrderBy(_ => Random.Shared.Next()).Take(total).ToList();
        var lista = new List<CartaEstado>();
        for (int i = 0; i < sel.Count; i++)
        {
            lista.Add(new CartaEstado(i, sel[i].Emoji, sel[i].Etiqueta));
            lista.Add(new CartaEstado(i, sel[i].Emoji, sel[i].Etiqueta));
        }
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (lista[i], lista[j]) = (lista[j], lista[i]);
        }
        return lista;
    }

    // ── Construir tablero visual ──────────────────────────────────────────────
    private void ConstruirTablero()
    {
        Tablero.Children.Clear();
        Tablero.RowDefinitions.Clear();
        Tablero.ColumnDefinitions.Clear();

        const int cols = 4;
        int filas = (int)Math.Ceiling(_cartas.Count / (double)cols);

        for (int c = 0; c < cols; c++)
            Tablero.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        for (int r = 0; r < filas; r++)
            Tablero.RowDefinitions.Add(new RowDefinition(new GridLength(88)));

        for (int i = 0; i < _cartas.Count; i++)
        {
            var frame = CrearFrameCarta(i);
            Grid.SetRow(frame, i / cols);
            Grid.SetColumn(frame, i % cols);
            Tablero.Children.Add(frame);
            _frames.Add(frame);
        }
    }

    private Frame CrearFrameCarta(int indice)
    {
        var dorso = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Children = { new Label { Text = "?", FontSize = 26, FontFamily = "NunitoBold", TextColor = ColDorsoTexto, HorizontalOptions = LayoutOptions.Center } }
        };

        var cara = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 2,
            Opacity = 0,
            Children =
            {
                new Label { Text = _cartas[indice].Emoji, FontSize = 26, HorizontalOptions = LayoutOptions.Center },
                new Label { Text = _cartas[indice].Etiqueta, FontSize = 10, FontFamily = "NunitoSemiBold",
                            TextColor = Color.FromArgb("#6B7280"), HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center, LineBreakMode = LineBreakMode.WordWrap,
                            MaximumWidthRequest = 68 }
            }
        };

        var frame = new Frame
        {
            BackgroundColor = ColDorsoFondo,
            CornerRadius = 14,
            Padding = 4,
            HasShadow = false,
            BorderColor = ColBorde,
            Content = new Grid { Children = { dorso, cara } },
            BindingContext = new CartaReferencias(dorso, cara)
        };

        var tap = new TapGestureRecognizer();
        tap.Tapped += async (_, _) => await ManejarToque(indice);
        frame.GestureRecognizers.Add(tap);
        return frame;
    }

    // ── Lógica del juego ──────────────────────────────────────────────────────
    private async Task ManejarToque(int indice)
    {
        var carta = _cartas[indice];
        if (_bloqueado || carta.Volteada || carta.Emparejada) return;

        await AnimarVolteo(_frames[indice], true);
        carta.Volteada = true;
        _esperando.Add(indice);

        if (_esperando.Count < 2) return;

        _bloqueado = true;
        _intentos++;
        LblIntentos.Text = _intentos.ToString();

        int i1 = _esperando[0], i2 = _esperando[1];

        if (_cartas[i1].PairId == _cartas[i2].PairId)
        {
            _cartas[i1].Emparejada = _cartas[i2].Emparejada = true;
            AplicarMatch(_frames[i1]); AplicarMatch(_frames[i2]);
            _paresEncontrados++;
            LblPares.Text = $"{_paresEncontrados}/{_totalPares}";
            _esperando.Clear(); _bloqueado = false;
            if (_paresEncontrados == _totalPares) MostrarVictoria();
        }
        else
        {
            await Task.Delay(950);
            await AnimarVolteo(_frames[i1], false);
            await AnimarVolteo(_frames[i2], false);
            _cartas[i1].Volteada = _cartas[i2].Volteada = false;
            _esperando.Clear(); _bloqueado = false;
        }
    }

    private static async Task AnimarVolteo(Frame frame, bool mostrar)
    {
        var refs = (CartaReferencias)frame.BindingContext!;
        await frame.ScaleXTo(0, 110, Easing.Linear);
        if (mostrar) { frame.BackgroundColor = ColCaraFondo; refs.Dorso.Opacity = 0; refs.Cara.Opacity = 1; }
        else { frame.BackgroundColor = ColDorsoFondo; refs.Dorso.Opacity = 1; refs.Cara.Opacity = 0; }
        await frame.ScaleXTo(1, 110, Easing.Linear);
    }

    private static void AplicarMatch(Frame frame)
    {
        frame.BackgroundColor = ColMatchFondo;
        frame.BorderColor = ColMatchBorde;
        var refs = (CartaReferencias)frame.BindingContext!;
        if (refs.Cara.Children.Count > 1 && refs.Cara.Children[1] is Label lbl)
            lbl.TextColor = ColMatchTexto;
    }

    private void MostrarVictoria()
    {
        _timer?.Stop();
        LblResultado.Text = $"Terminaste en {_segundos}s con {_intentos} intentos 🌟";
        BannerVictoria.IsVisible = true;
    }

    // ── Eventos de UI ─────────────────────────────────────────────────────────
    private void OnNuevaPartida(object sender, EventArgs e) => IniciarJuego();

    // Volver al menú de juegos
    private async void OnVolverTocado(object sender, TappedEventArgs e)
    {
        _timer?.Stop();
        await Navigation.PopAsync();
    }

    private void OnDificultadFacil(object sender, TappedEventArgs e)
    { _totalPares = 8; ActivarBtn(BtnFacil); IniciarJuego(); }
    private void OnDificultadMedio(object sender, TappedEventArgs e)
    { _totalPares = 12; ActivarBtn(BtnMedio); IniciarJuego(); }
    private void OnDificultadDificil(object sender, TappedEventArgs e)
    { _totalPares = 16; ActivarBtn(BtnDificil); IniciarJuego(); }

    private void ActivarBtn(Frame activo)
    {
        foreach (var btn in new[] { BtnFacil, BtnMedio, BtnDificil })
        {
            btn.BackgroundColor = Colors.White;
            if (btn.Content is VerticalStackLayout vsl)
                foreach (var lbl in vsl.Children.OfType<Label>())
                    lbl.TextColor = Color.FromArgb("#9CA3AF");
        }
        activo.BackgroundColor = ColDorsoFondo;
        if (activo.Content is VerticalStackLayout avsl)
            foreach (var lbl in avsl.Children.OfType<Label>())
                lbl.TextColor = ColDorsoTexto;
    }
}
