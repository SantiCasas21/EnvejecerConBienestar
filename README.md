# 🌿 Envejecer con Bienestar — Prototipo .NET MAUI

App móvil para adultos mayores. iOS + Android desde un solo codebase en .NET 10 MAUI.

---

## 📋 Requisitos previos

| Herramienta | Versión mínima | Verificar con |
|---|---|---|
| .NET SDK | **10.0** | `dotnet --version` |
| .NET MAUI workload | 10.x | `dotnet workload list` |
| Android SDK | API 21+ | Android Studio SDK Manager |
| Xcode (solo macOS/iOS) | 15+ | `xcode-select -p` |
| Visual Studio / Rider | 2022 17.10+ / 2024.1+ | — |

### Instalar el workload de MAUI (si aún no está)
```bash
dotnet workload install maui
```

---

## ⚡ Configuración inicial — Fuentes Nunito

La app usa la fuente **Nunito** (Google Fonts, licencia OFL).
Debe agregar los archivos `.ttf` manualmente:

1. Descargue desde https://fonts.google.com/specimen/Nunito
2. Extraiga y copie estos tres archivos a `Resources/Fonts/`:
   - `Nunito-Regular.ttf`
   - `Nunito-SemiBold.ttf`
   - `Nunito-Bold.ttf`

> **Atajo rápido:** Si no tiene las fuentes a mano, en `MauiProgram.cs` puede comentar
> las tres líneas `fonts.AddFont(...)` y la app compilará usando la fuente del sistema.
> El diseño se verá casi idéntico.

---

## 🚀 Compilar y ejecutar

### Android (emulador o dispositivo físico)
```bash
# Emulador (asegúrese de tener uno creado en AVD Manager)
dotnet build -t:Run -f net10.0-android

# Dispositivo físico (conectado por USB con depuración activada)
dotnet build -t:Run -f net10.0-android -p:AndroidAttachDebugger=false
```

### iOS (solo macOS)
```bash
# Simulador
dotnet build -t:Run -f net10.0-ios -p:_DeviceName=:v2:udid=<UDID_SIMULADOR>

# Listar simuladores disponibles
xcrun simctl list devices available
```

### Desde Visual Studio / Rider
1. Abrir `EnvejecerConBienestar.sln` (o el `.csproj`)
2. Seleccionar el framework de destino (`net10.0-android` o `net10.0-ios`)
3. Presionar **Run / F5**

---

## 🗂️ Estructura del proyecto

```
EnvejecerConBienestar/
│
├── Models/
│   └── Medicamento.cs          ← Modelo de datos (nombre, dosis, hora, tomado)
│
├── ViewModels/
│   ├── HomeViewModel.cs        ← Saludo dinámico + comando emergencia
│   └── MedicamentosViewModel.cs ← Lista reactiva + toggle tomado/pendiente
│
├── Views/
│   ├── HomePage.xaml / .cs     ← Pantalla inicio (saludo + emergencia)
│   ├── MedicamentosPage.xaml / .cs ← Lista con CollectionView + convertidores
│   ├── JuegosPage.xaml / .cs   ← Cascarón "Próximamente"
│   └── ContactosPage.xaml / .cs ← Cascarón "Próximamente"
│
├── Resources/
│   ├── Fonts/                  ← Nunito-Regular/SemiBold/Bold.ttf (agregar manualmente)
│   ├── Styles/
│   │   ├── Colors.xaml         ← Paleta de colores cálidos completa
│   │   └── Styles.xaml         ← Estilos globales (botones, labels, tarjetas)
│   ├── Images/                 ← Íconos de la TabBar (agregar PNGs)
│   └── Splash/                 ← Splash screen
│
├── AppShell.xaml               ← Navegación inferior (4 pestañas)
├── App.xaml / .cs
└── MauiProgram.cs              ← DI container, registro de fuentes
```

---

## 🎨 Paleta de colores

| Color | Hex | Uso |
|---|---|---|
| Naranja cálido | `#F97316` | Primario, botones de acción |
| Crema suave | `#FFF7ED` | Fondo de todas las pantallas |
| Verde salud | `#0D9488` | Botón "Marcar tomado", progreso |
| Rojo emergencia | `#DC2626` | **Solo** para el botón de emergencia |
| Violeta juegos | `#7C3AED` | Sección Juegos |
| Azul contactos | `#1D4ED8` | Sección Contactos |

---

## 🖼️ Íconos de la TabBar

La app referencia estos archivos PNG en `Resources/Images/`:
- `tab_inicio.png`
- `tab_medicina.png`
- `tab_juegos.png`
- `tab_contactos.png`

**Para el prototipo del sábado**, puede usar cualquier PNG de 32×32px o simplemente
eliminar el atributo `Icon="..."` de cada `<ShellContent>` en `AppShell.xaml`
y la TabBar mostrará solo los títulos de texto.

---

## 🏗️ División de trabajo sugerida (equipo de 3)

| Desarrollador | Tarea para el sábado |
|---|---|
| **Dev 1** | Pantalla Inicio (pulir animaciones, clima real) + AppShell |
| **Dev 2** | Pantalla Medicamentos (persistencia SQLite, notificaciones locales) |
| **Dev 3** | Empezar Juegos (Memorama o Sudoku básico) + Contactos básicos |

---

## 🔮 Próximos pasos (Fase 2)

- [ ] Persistencia local con **SQLite-net-pcl**
- [ ] Notificaciones locales con **Plugin.LocalNotification**
- [ ] Integración **HealthKit** (iOS) / **Health Connect** (Android)
- [ ] Pantalla Juegos: Sudoku, Sopa de letras, Memorama
- [ ] Pantalla Contactos: llamada directa, envío GPS de emergencia
- [ ] Pantalla Salud: gráficas de presión y glucosa
- [ ] Sincronización en la nube (Azure / Firebase)

---

## 🐛 Problemas comunes

**Error: `MAUI workload not found`**
```bash
dotnet workload install maui --source https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet10/nuget/v3/index.json
```

**Error: fuentes no encontradas en runtime**
→ Verificar que los `.ttf` estén en `Resources/Fonts/` y su Build Action sea `MauiFont`.
En VS: click derecho sobre el archivo → Propiedades → Build Action = `MauiFont`.

**Error: `BoolToColorConverter not found`**
→ Asegurarse de que el namespace `xmlns:local` en `MedicamentosPage.xaml` apunte a
`clr-namespace:EnvejecerConBienestar.Views`.

---

*Generado para el equipo — prototipo funcional para demo del sábado 🚀*
