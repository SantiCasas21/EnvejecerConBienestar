namespace EnvejecerConBienestar.Views;

public partial class TriviaPage : ContentPage
{
    // ═══════════════════════════════════════════════════════════════
    //                    MODELO DE PREGUNTA
    // ═══════════════════════════════════════════════════════════════
    private sealed class Pregunta
    {
        public string Emoji { get; init; } = "";
        public string Texto { get; init; } = "";
        public string[] Opciones { get; init; } = Array.Empty<string>();
        public int RespuestaCorrecta { get; init; }   // índice 0-3
        public string Explicacion { get; init; } = "";
    }

    // ═══════════════════════════════════════════════════════════════
    //                    BANCO DE PREGUNTAS
    // ═══════════════════════════════════════════════════════════════
    private readonly List<Pregunta> _banco = new()
    {
        new Pregunta
        {
            Emoji = "💧", Texto = "¿Cuántos vasos de agua se recomienda beber al día?",
            Opciones = new[]{ "2 vasos", "4 vasos", "8 vasos", "12 vasos" },
            RespuestaCorrecta = 2,
            Explicacion = "8 vasos (2 litros aprox.) es la cantidad recomendada para mantenerse hidratado."
        },
        new Pregunta
        {
            Emoji = "🚶", Texto = "¿Cuántos minutos de caminata diaria se recomiendan para adultos mayores?",
            Opciones = new[]{ "5 minutos", "15 minutos", "30 minutos", "2 horas" },
            RespuestaCorrecta = 2,
            Explicacion = "30 minutos de caminata moderada al día mejora el corazón y el estado de ánimo."
        },
        new Pregunta
        {
            Emoji = "🥦", Texto = "¿Cuántas porciones de frutas y verduras se deben comer al día?",
            Opciones = new[]{ "1 porción", "2 porciones", "3 porciones", "5 porciones" },
            RespuestaCorrecta = 3,
            Explicacion = "5 porciones al día aportan vitaminas, minerales y fibra esenciales."
        },
        new Pregunta
        {
            Emoji = "😴", Texto = "¿Cuántas horas de sueño necesitan los adultos mayores cada noche?",
            Opciones = new[]{ "3-4 horas", "5-6 horas", "7-8 horas", "10-12 horas" },
            RespuestaCorrecta = 2,
            Explicacion = "Dormir 7-8 horas mejora la memoria, el ánimo y el sistema inmune."
        },
        new Pregunta
        {
            Emoji = "☀️", Texto = "¿Qué vitamina produce el cuerpo al exponerse al sol?",
            Opciones = new[]{ "Vitamina A", "Vitamina B12", "Vitamina C", "Vitamina D" },
            RespuestaCorrecta = 3,
            Explicacion = "La Vitamina D fortalece los huesos y se produce con 15-20 min de sol al día."
        },
        new Pregunta
        {
            Emoji = "❤️", Texto = "¿Cuál de estos alimentos es mejor para el corazón?",
            Opciones = new[]{ "Embutidos", "Aceite de oliva", "Mantequilla", "Frituras" },
            RespuestaCorrecta = 1,
            Explicacion = "El aceite de oliva contiene grasas saludables que protegen el corazón."
        },
        new Pregunta
        {
            Emoji = "🧠", Texto = "¿Qué actividad ayuda más a ejercitar la memoria?",
            Opciones = new[]{ "Ver televisión todo el día", "Leer, jugar o aprender algo nuevo", "Dormir mucho", "Evitar hablar con otros" },
            RespuestaCorrecta = 1,
            Explicacion = "Leer, jugar y aprender estimulan conexiones en el cerebro y mejoran la memoria."
        },
        new Pregunta
        {
            Emoji = "🩺", Texto = "¿Con qué frecuencia se recomienda una revisión médica preventiva?",
            Opciones = new[]{ "Solo cuando duele algo", "Cada 5 años", "Una vez al año", "Nunca si uno se siente bien" },
            RespuestaCorrecta = 2,
            Explicacion = "Una revisión anual permite detectar enfermedades a tiempo, aunque uno se sienta bien."
        },
        new Pregunta
        {
            Emoji = "🧘", Texto = "¿Qué beneficio tiene practicar ejercicios de respiración o meditación?",
            Opciones = new[]{ "Aumenta la presión arterial", "Reduce el estrés y la ansiedad", "Provoca insomnio", "No tiene ningún efecto" },
            RespuestaCorrecta = 1,
            Explicacion = "La meditación y respiración profunda activan el sistema nervioso parasimpático, reduciendo el estrés."
        },
        new Pregunta
        {
            Emoji = "🤝", Texto = "¿Por qué es importante el contacto social en la vejez?",
            Opciones = new[]{ "No es importante", "Previene la depresión y el deterioro cognitivo", "Solo sirve para distraerse", "Puede ser dañino" },
            RespuestaCorrecta = 1,
            Explicacion = "Las relaciones sociales reducen el riesgo de depresión y estimulan el cerebro."
        },
    };

    // ═══════════════════════════════════════════════════════════════
    //                      ESTADO DEL JUEGO
    // ═══════════════════════════════════════════════════════════════
    private List<Pregunta> _preguntas = new();
    private int _indice = 0;
    private int _puntaje = 0;
    private bool _respondida = false;
    private bool _juegoFinalizado = false;

    // Referencias a los 4 frames de opciones y sus badges
    private Frame[] _frames = Array.Empty<Frame>();
    private Frame[] _badges = Array.Empty<Frame>();
    private Label[] _labels = Array.Empty<Label>();

    // Colores para feedback
    private static readonly Color ColorCorrecto = Color.FromArgb("#D1FAE5");
    private static readonly Color ColorIncorrecto = Color.FromArgb("#FEE2E2");
    private static readonly Color ColorBordeCorrecto = Color.FromArgb("#6EE7B7");
    private static readonly Color ColorBordeIncorrecto = Color.FromArgb("#FCA5A5");
    private static readonly Color ColorBadgeCorrecto = Color.FromArgb("#059669");
    private static readonly Color ColorBadgeIncorrecto = Color.FromArgb("#DC2626");
    private static readonly Color ColorTextoCorrecto = Color.FromArgb("#065F46");
    private static readonly Color ColorTextoIncorrecto = Color.FromArgb("#991B1B");
    private static readonly Color ColorDefault = Color.FromArgb("#F3F4F6");
    private static readonly Color ColorBordeDefault = Color.FromArgb("#F3F4F6");
    private static readonly Color ColorTextoDefault = Color.FromArgb("#6B7280");

    // ═══════════════════════════════════════════════════════════════
    public TriviaPage()
    {
        InitializeComponent();

        // Guardar referencias a los 4 frames de opciones
        _frames = new[] { OpcionA, OpcionB, OpcionC, OpcionD };
        _badges = new[] { BadgeA, BadgeB, BadgeC, BadgeD };
        _labels = new[] { LblOpcionA, LblOpcionB, LblOpcionC, LblOpcionD };

        IniciarJuego();
    }

    // ═══════════════════════════════════════════════════════════════
    //                     INICIAR / REINICIAR
    // ═══════════════════════════════════════════════════════════════
    private void IniciarJuego()
    {
        _indice = 0;
        _puntaje = 0;
        _respondida = false;
        _juegoFinalizado = false;

        // Mezclar preguntas
        _preguntas = _banco.OrderBy(_ => Random.Shared.Next()).ToList();

        // Ocultar paneles de resultado
        BannerFeedback.IsVisible = false;
        PanelFinal.IsVisible = false;
        BtnAccion.IsVisible = false;

        // Mostrar opciones
        foreach (var f in _frames) f.IsVisible = true;

        MostrarPregunta();
    }

    // ═══════════════════════════════════════════════════════════════
    //                      MOSTRAR PREGUNTA
    // ═══════════════════════════════════════════════════════════════
    private void MostrarPregunta()
    {
        _respondida = false;
        var pregunta = _preguntas[_indice];

        // Actualizar encabezado
        int numero = _indice + 1;
        int total = _preguntas.Count;
        LblProgreso.Text = $"Pregunta {numero} de {total}";
        BarraProgreso.Progress = (double)numero / total;
        LblPuntaje.Text = $"{_puntaje} pts";

        // Contenido de la pregunta
        LblEmojiPregunta.Text = pregunta.Emoji;
        LblPregunta.Text = pregunta.Texto;

        // Rellenar opciones y resetear estilos
        for (int i = 0; i < 4; i++)
        {
            _labels[i].Text = pregunta.Opciones[i];
            _labels[i].TextColor = Color.FromArgb("#1F2937");

            _frames[i].BackgroundColor = Colors.White;
            _frames[i].BorderColor = ColorBordeDefault;

            _badges[i].BackgroundColor = ColorDefault;
            if (_badges[i].Content is Label badgeLbl)
                badgeLbl.TextColor = ColorTextoDefault;
        }

        // Ocultar feedback anterior
        BannerFeedback.IsVisible = false;
        BtnAccion.IsVisible = false;
    }

    // ═══════════════════════════════════════════════════════════════
    //                    MANEJAR RESPUESTA
    // ═══════════════════════════════════════════════════════════════
    private async void OnOpcionTocada(object sender, TappedEventArgs e)
    {
        if (_respondida || _juegoFinalizado) return;
        _respondida = true;

        int seleccion = int.Parse(e.Parameter?.ToString() ?? "0");
        var pregunta = _preguntas[_indice];
        bool correcta = seleccion == pregunta.RespuestaCorrecta;

        if (correcta) _puntaje += 10;

        // Colorear la opción seleccionada
        AplicarColorOpcion(seleccion, correcta);

        // Si se equivocó, mostrar también cuál era la correcta
        if (!correcta)
            AplicarColorOpcion(pregunta.RespuestaCorrecta, true);

        // Mostrar feedback
        MostrarFeedback(correcta, pregunta.Explicacion);

        // Pequeña pausa antes de mostrar el botón Siguiente
        await Task.Delay(400);
        BtnAccion.IsVisible = true;
        BtnAccion.Text = (_indice < _preguntas.Count - 1) ? "Siguiente →" : "Ver resultado";
    }

    private void AplicarColorOpcion(int idx, bool esCorrecto)
    {
        _frames[idx].BackgroundColor = esCorrecto ? ColorCorrecto : ColorIncorrecto;
        _frames[idx].BorderColor = esCorrecto ? ColorBordeCorrecto : ColorBordeIncorrecto;

        _badges[idx].BackgroundColor = esCorrecto ? ColorBadgeCorrecto : ColorBadgeIncorrecto;
        if (_badges[idx].Content is Label lbl)
        {
            lbl.TextColor = Colors.White;
            lbl.Text = esCorrecto ? "✓" : "✗";
        }

        _labels[idx].TextColor = esCorrecto ? ColorTextoCorrecto : ColorTextoIncorrecto;
    }

    private void MostrarFeedback(bool correcta, string explicacion)
    {
        if (correcta)
        {
            BannerFeedback.BackgroundColor = ColorCorrecto;
            BannerFeedback.BorderColor = ColorBordeCorrecto;
            LblFeedbackTitulo.Text = "✅ ¡Correcto! +10 puntos";
            LblFeedbackTitulo.TextColor = ColorTextoCorrecto;
            LblFeedbackDetalle.Text = explicacion;
            LblFeedbackDetalle.TextColor = ColorTextoCorrecto;
        }
        else
        {
            BannerFeedback.BackgroundColor = ColorIncorrecto;
            BannerFeedback.BorderColor = ColorBordeIncorrecto;
            LblFeedbackTitulo.Text = "❌ Incorrecto";
            LblFeedbackTitulo.TextColor = ColorTextoIncorrecto;
            LblFeedbackDetalle.Text = explicacion;
            LblFeedbackDetalle.TextColor = ColorTextoIncorrecto;
        }

        BannerFeedback.IsVisible = true;
    }

    // ═══════════════════════════════════════════════════════════════
    //                  BOTÓN SIGUIENTE / VER RESULTADO
    // ═══════════════════════════════════════════════════════════════
    private void OnAccionTocada(object sender, EventArgs e)
    {
        _indice++;

        if (_indice < _preguntas.Count)
        {
            MostrarPregunta();
        }
        else
        {
            MostrarResultadoFinal();
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //                      RESULTADO FINAL
    // ═══════════════════════════════════════════════════════════════
    private void MostrarResultadoFinal()
    {
        _juegoFinalizado = true;
        int total = _preguntas.Count;
        int correctas = _puntaje / 10;
        double porcentaje = (double)correctas / total;

        // Ocultar preguntas y feedback
        foreach (var f in _frames) f.IsVisible = false;
        BannerFeedback.IsVisible = false;
        BtnAccion.IsVisible = false;

        // Emoji y mensaje según rendimiento
        if (porcentaje >= 0.8)
        {
            LblEmojiFinal.Text = "🏆";
            LblPuntajeFinal.Text = $"¡Excelente! Respondiste {correctas} de {total} correctamente.\nPuntaje: {_puntaje} / {total * 10} pts";
        }
        else if (porcentaje >= 0.5)
        {
            LblEmojiFinal.Text = "👍";
            LblPuntajeFinal.Text = $"¡Bien hecho! Respondiste {correctas} de {total} correctamente.\nPuntaje: {_puntaje} / {total * 10} pts";
        }
        else
        {
            LblEmojiFinal.Text = "💪";
            LblPuntajeFinal.Text = $"¡Sigue practicando! Respondiste {correctas} de {total} correctamente.\nPuntaje: {_puntaje} / {total * 10} pts";
        }

        LblProgreso.Text = $"Resultado final";
        LblPuntaje.Text = $"{_puntaje} pts";
        PanelFinal.IsVisible = true;

        // Reusar el botón para jugar de nuevo
        BtnAccion.Text = "🔄 Jugar de nuevo";
        BtnAccion.IsVisible = true;
        BtnAccion.Clicked -= OnAccionTocada;
        BtnAccion.Clicked += (_, _) => IniciarJuego();
    }

    // ═══════════════════════════════════════════════════════════════
    //                      EVENTOS DE UI
    // ═══════════════════════════════════════════════════════════════
    private async void OnVolverTocado(object sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("..");
}
