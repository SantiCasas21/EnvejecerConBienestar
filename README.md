# 🌿 Envejecer con Bienestar

![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Android](https://img.shields.io/badge/Android-3DDC84?style=for-the-badge&logo=android&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)

**Envejecer con Bienestar** es una aplicación móvil desarrollada con **.NET MAUI** diseñada específicamente para mejorar la calidad de vida de los adultos mayores. Este proyecto combina tecnología de vanguardia con un diseño centrado en el usuario para facilitar la gestión de la salud y el bienestar diario.

---

## ✨ Características Principales

* **Gestión de Medicamentos:** Recordatorios visuales y sonoros para la toma de medicinas.
* **Interfaz Adaptada:** Diseño con alto contraste, tipografía legible (Nunito) y elementos táctiles simplificados.
* **Dashboard Personalizado:** Saludo dinámico y acceso rápido a las funciones más importantes.
* **Multiplataforma:** Compilado inicialmente para Android, con capacidad de despliegue en Windows e iOS.

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

## 🛠️ Stack Tecnológico

* **Framework:** .NET 9.0 (MAUI)
* **Lenguaje:** C#
* **Arquitectura:** MVVM (Model-View-ViewModel) para una separación clara de la lógica y la interfaz.
* **UI/UX:** XAML con estilos personalizados para accesibilidad.

## 🚀 Instalación y Ejecución

1. Clonar el repositorio:
   ```bash
   git clone [https://github.com/tu-usuario/EnvejecerConBienestar.git](https://github.com/tu-usuario/EnvejecerConBienestar.git)

---
