namespace EnvejecerConBienestar.Views;

public partial class OrdenarSecuenciaPage : ContentPage
{
    // ═══════════════════════════════════════════════════════════════════════
    //   MODELO DE RUTINA
    // ═══════════════════════════════════════════════════════════════════════
    private sealed record Rutina(
        string Emoji,
        string Titulo,
        string[] PasosCorrectos   // en el orden correcto
    );

    // ═══════════════════════════════════════════════════════════════════════
    //   BANCO DE RUTINAS (4 rutinas temáticas sobre salud)
    // ═══════════════════════════════════════════════════════════════════════
    private static readonly Rutina[] RUTINAS = new[]
    {
        new Rutina("🍳", "Preparar el desayuno", new[]
        {
            "Lavarse las manos antes de cocinar",
            "Lavar las frutas y verduras",
            "Preparar y servir los alimentos",
            "Sentarse en la mesa con calma",
            "Comer despacio y masticar bien",
        }),
        new Rutina("🏃", "Rutina de ejercicio", new[]
        {
            "Ponerse ropa y zapatos cómodos",
            "Calentar los músculos suavemente",
            "Realizar los ejercicios principales",
            "Caminar a paso moderado",
            "Estirar el cuerpo y descansar",
        }),
        new Rutina("💊", "Tomar los medicamentos", new[]
        {
            "Verificar el nombre del medicamento",
            "Revisar la dosis indicada por el médico",
            "Tomar el medicamento con un vaso de agua",
            "Anotar la hora en que lo tomó",
            "Guardar los medicamentos en lugar seguro",
        }),
        new Rutina("😴", "Prepararse para dormir", new[]
        {
            "Apagar la televisión y el teléfono",
            "Ponerse el pijama cómodamente",
            "Tomar agua o leche tibia",
            "Leer un poco o escuchar música suave",
            "Apagar la luz y acostarse",
        }),
    };

    // ═══════════════════════════════════════════════════════════════════════
    //   ESTADO DEL JUEGO
    // ═══════════════════════════════════════════════════════════════════════

    // Clase auxiliar que guarda las referencias UI de cada tarjeta de paso
    private sealed class TarjetaPaso
    {
        public Frame FramePrincipal { get; init; } = null!;
        public Frame Badge { get; init; } = null!;   // círculo con número
        public Label BadgeLbl { get; init; } = null!;   // número asignado
        public Label TextoLbl { get; init; } = null!;   // texto del paso
        public string TextoPaso { get; init; } = "";      // texto original
        public int PosicionUsuario { get; set; } = 0;       // 0 = sin asignar
    }

    private List<TarjetaPaso> _tarjetas = new();
    private int _rutinaActual = 0;
    private int _puntosTotal = 0;
    private int _siguienteNum = 1;    // siguiente número a asignar
    private bool _verificado = false;

    // ─── Colores ──────────────────────────────────────────────────────────
    private static readonly Color ColCardNormal = Colors.White;
    private static readonly Color ColCardSelected = Color.FromArgb("#F0FDF4");
    private static readonly Color ColBordeNormal = Color.FromArgb("#F3F4F6");
    private static readonly Color ColBordeSelected = Color.FromArgb("#0D9488");
    private static readonly Color ColBadgeVacio = Color.FromArgb("#F3F4F6");
    private static readonly Color ColBadgeAsignado = Color.FromArgb("#0D9488");
    private static readonly Color ColBadgeCorrecto = Color.FromArgb("#059669");
    private static readonly Color ColBadgeError = Color.FromArgb("#DC2626");
    private static readonly Color ColTextoNormal = Color.FromArgb("#1F2937");
    private static readonly Color ColTextoSecund = Color.FromArgb("#6B7280");

    // ═══════════════════════════════════════════════════════════════════════
    public OrdenarSecuenciaPage()
    {
        InitializeComponent();
        CargarRutina(_rutinaActual);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   CARGAR RUTINA
    // ═══════════════════════════════════════════════════════════════════════
    private void CargarRutina(int indice)
    {
        _verificado = false;
        _siguienteNum = 1;
        _tarjetas.Clear();

        var rutina = RUTINAS[indice];

        // ─ Actualizar encabezado ──────────────────────────────────────────
        LblRutina.Text = $"Rutina {indice + 1} de {RUTINAS.Length}";
        LblEmojiRutina.Text = rutina.Emoji;
        LblTituloRutina.Text = rutina.Titulo;
        LblPuntos.Text = $"{_puntosTotal} pts";
        BarraProgreso.Progress = (double)(indice + 1) / RUTINAS.Length;
        LblInstruccion.Text = "Toca el 1er paso → luego el 2do → y así hasta completar";

        // ─ Ocultar botones y banners ─────────────────────────────────────
        BtnVerificar.IsVisible = false;
        BannerResultado.IsVisible = false;
        BtnSiguiente.IsVisible = false;
        BtnReintentar.IsVisible = false;

        // ─ Barajar los pasos ─────────────────────────────────────────────
        var pasosBarajados = rutina.PasosCorrectos
            .OrderBy(_ => Random.Shared.Next())
            .ToList();

        // ─ Generar tarjetas ──────────────────────────────────────────────
        ContenedorPasos.Children.Clear();

        foreach (var paso in pasosBarajados)
        {
            string textoPaso = paso;   // captura para el closure

            // Badge circular con número (vacío al inicio)
            var badgeLbl = new Label
            {
                Text = "",
                FontFamily = "NunitoBold",
                FontSize = 15,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };

            var badge = new Frame
            {
                BackgroundColor = ColBadgeVacio,
                CornerRadius = 20,
                HeightRequest = 36,
                WidthRequest = 36,
                Padding = 0,
                HasShadow = false,
                Content = badgeLbl,
                VerticalOptions = LayoutOptions.Center,
            };

            // Texto del paso
            var textoLbl = new Label
            {
                Text = textoPaso,
                FontFamily = "NunitoSemiBold",
                FontSize = 14,
                TextColor = ColTextoNormal,
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.WordWrap,
            };

            var contenido = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition(new GridLength(44)),
                    new ColumnDefinition(GridLength.Star),
                },
                ColumnSpacing = 10,
                Children = { badge, textoLbl },
            };
            Grid.SetColumn(badge, 0);
            Grid.SetColumn(textoLbl, 1);

            var frame = new Frame
            {
                BackgroundColor = ColCardNormal,
                CornerRadius = 16,
                Padding = new Thickness(14, 14),
                HasShadow = false,
                BorderColor = ColBordeNormal,
                Content = contenido,
            };

            // Registrar tarjeta
            var tarjeta = new TarjetaPaso
            {
                FramePrincipal = frame,
                Badge = badge,
                BadgeLbl = badgeLbl,
                TextoLbl = textoLbl,
                TextoPaso = textoPaso,
                PosicionUsuario = 0,
            };
            _tarjetas.Add(tarjeta);

            // Gesto de toque
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, _) => OnPasoTocado(tarjeta);
            frame.GestureRecognizers.Add(tap);

            ContenedorPasos.Children.Add(frame);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   LÓGICA DE TOQUE SOBRE UN PASO
    // ═══════════════════════════════════════════════════════════════════════
    private void OnPasoTocado(TarjetaPaso tocada)
    {
        if (_verificado) return;

        if (tocada.PosicionUsuario > 0)
        {
            // ─ Ya estaba asignado → deseleccionar esta y todas las posteriores ─
            int desde = tocada.PosicionUsuario;

            foreach (var t in _tarjetas.Where(t => t.PosicionUsuario >= desde))
            {
                t.PosicionUsuario = 0;
                AplicarEstilo(t, "normal");
            }

            _siguienteNum = desde;
        }
        else
        {
            // ─ Sin asignar → asignar el siguiente número ──────────────────
            tocada.PosicionUsuario = _siguienteNum;
            AplicarEstilo(tocada, "asignado");
            _siguienteNum++;
        }

        // ¿Todos los pasos tienen número? → mostrar botón Verificar
        bool todosAsignados = _tarjetas.All(t => t.PosicionUsuario > 0);
        BtnVerificar.IsVisible = todosAsignados;

        if (todosAsignados)
            LblInstruccion.Text = "¡Listo! Toca \"Verificar\" para comprobar tu respuesta";
        else
            LblInstruccion.Text = $"Selecciona el paso número {_siguienteNum}...";
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   VERIFICAR EL ORDEN
    // ═══════════════════════════════════════════════════════════════════════
    private void OnVerificarTocado(object sender, EventArgs e)
    {
        _verificado = true;
        BtnVerificar.IsVisible = false;

        var rutina = RUTINAS[_rutinaActual];
        int totalPasos = rutina.PasosCorrectos.Length;
        int correctos = 0;

        // Ordenar tarjetas según la posición que asignó el usuario
        var ordenUsuario = _tarjetas.OrderBy(t => t.PosicionUsuario).ToList();

        for (int i = 0; i < totalPasos; i++)
        {
            bool esCorrecto = ordenUsuario[i].TextoPaso == rutina.PasosCorrectos[i];

            if (esCorrecto)
            {
                correctos++;
                AplicarEstilo(ordenUsuario[i], "correcto");
            }
            else
            {
                AplicarEstilo(ordenUsuario[i], "error");
            }
        }

        bool perfecto = correctos == totalPasos;
        int puntos = perfecto ? 20 : correctos * 4;
        _puntosTotal += puntos;
        LblPuntos.Text = $"{_puntosTotal} pts";

        // ─ Mostrar banner de resultado ────────────────────────────────────
        MostrarResultado(perfecto, correctos, totalPasos, puntos);
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   MOSTRAR RESULTADO
    // ═══════════════════════════════════════════════════════════════════════
    private void MostrarResultado(bool perfecto, int correctos, int total, int puntos)
    {
        BannerResultado.IsVisible = true;

        if (perfecto)
        {
            BannerResultado.BackgroundColor = Color.FromArgb("#D1FAE5");
            BannerResultado.BorderColor = Color.FromArgb("#6EE7B7");
            LblResultadoTitulo.Text = "🎉 ¡Perfecto! +20 puntos";
            LblResultadoTitulo.TextColor = Color.FromArgb("#065F46");
            LblResultadoDetalle.Text = "¡Ordenaste todos los pasos correctamente!";
            LblResultadoDetalle.TextColor = Color.FromArgb("#047857");
        }
        else
        {
            BannerResultado.BackgroundColor = Color.FromArgb("#FEF3C7");
            BannerResultado.BorderColor = Color.FromArgb("#FCD34D");
            LblResultadoTitulo.Text = $"⚠️ {correctos} de {total} correctos · +{puntos} pts";
            LblResultadoTitulo.TextColor = Color.FromArgb("#92400E");
            LblResultadoDetalle.Text =
                "Las tarjetas en verde están bien. " +
                "Las rojas no estaban en el lugar correcto.";
            LblResultadoDetalle.TextColor = Color.FromArgb("#92400E");
        }

        // ─ Mostrar botones de acción ──────────────────────────────────────
        bool hayMasRutinas = _rutinaActual < RUTINAS.Length - 1;

        if (perfecto && hayMasRutinas)
        {
            BtnSiguiente.IsVisible = true;
            BtnReintentar.IsVisible = false;
        }
        else if (perfecto && !hayMasRutinas)
        {
            // Completó todas las rutinas
            BtnSiguiente.Text = "🔄 Jugar de nuevo";
            BtnSiguiente.IsVisible = true;
        }
        else
        {
            // No fue perfecto: ofrecer reintentar y/o saltar
            BtnReintentar.IsVisible = true;
            if (hayMasRutinas)
                BtnSiguiente.IsVisible = true;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   ESTILOS VISUALES DE LAS TARJETAS
    // ═══════════════════════════════════════════════════════════════════════
    private static void AplicarEstilo(TarjetaPaso t, string estado)
    {
        switch (estado)
        {
            case "asignado":
                t.FramePrincipal.BackgroundColor = ColCardSelected;
                t.FramePrincipal.BorderColor = ColBordeSelected;
                t.Badge.BackgroundColor = ColBadgeAsignado;
                t.BadgeLbl.Text = t.PosicionUsuario.ToString();
                t.BadgeLbl.TextColor = Colors.White;
                t.TextoLbl.TextColor = ColTextoNormal;
                break;

            case "correcto":
                t.FramePrincipal.BackgroundColor = Color.FromArgb("#D1FAE5");
                t.FramePrincipal.BorderColor = Color.FromArgb("#6EE7B7");
                t.Badge.BackgroundColor = ColBadgeCorrecto;
                t.BadgeLbl.Text = t.PosicionUsuario.ToString();
                t.BadgeLbl.TextColor = Colors.White;
                t.TextoLbl.TextColor = Color.FromArgb("#065F46");
                break;

            case "error":
                t.FramePrincipal.BackgroundColor = Color.FromArgb("#FEE2E2");
                t.FramePrincipal.BorderColor = Color.FromArgb("#FCA5A5");
                t.Badge.BackgroundColor = ColBadgeError;
                t.BadgeLbl.Text = t.PosicionUsuario.ToString();
                t.BadgeLbl.TextColor = Colors.White;
                t.TextoLbl.TextColor = Color.FromArgb("#991B1B");
                break;

            default: // "normal"
                t.FramePrincipal.BackgroundColor = ColCardNormal;
                t.FramePrincipal.BorderColor = ColBordeNormal;
                t.Badge.BackgroundColor = ColBadgeVacio;
                t.BadgeLbl.Text = "";
                t.TextoLbl.TextColor = ColTextoNormal;
                t.PosicionUsuario = 0;
                break;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    //   EVENTOS DE BOTONES
    // ═══════════════════════════════════════════════════════════════════════

    // Siguiente rutina
    private void OnSiguienteTocado(object sender, EventArgs e)
    {
        _rutinaActual++;

        if (_rutinaActual >= RUTINAS.Length)
        {
            // Completó todas → reiniciar desde la primera
            _rutinaActual = 0;
            _puntosTotal = 0;
        }

        CargarRutina(_rutinaActual);
    }

    // Reintentar la rutina actual
    private void OnReintentarTocado(object sender, EventArgs e)
        => CargarRutina(_rutinaActual);

    // Volver al menú de juegos
    private async void OnVolverTocado(object sender, TappedEventArgs e)
        => await Navigation.PopAsync();
}
