# 🌿 Envejecer con Bienestar

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/en-us/apps/maui)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white)](https://www.sqlite.org/index.html)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

**Envejecer con Bienestar** es una solución móvil integral diseñada para mejorar la autonomía y calidad de vida de los adultos mayores. Desarrollada con **.NET MAUI**, la aplicación combina una interfaz altamente accesible con herramientas potentes de gestión de salud, estimulación cognitiva y seguridad.

---

## ✨ Características Principales

### 💊 Gestión de Salud y Medicamentos
* **Recordatorios Inteligentes:** Notificaciones locales programables para asegurar que nunca se olvide una toma.
* **Control de Inventario:** Seguimiento automático de dosis restantes con alertas de "umbral bajo" para reposición de medicinas.
* **Historial de Tomas:** Registro visual (✅/⏳) para verificar el cumplimiento del tratamiento diario.

### 🧠 Estimulación Cognitiva (Centro de Juegos)
* **Juegos Integrados:** Incluye actividades diseñadas para mantener la mente activa:
    * **Trivia:** Desafíos de conocimiento general.
    * **Sopa de Letras:** Ejercicios de agudeza visual.
    * **Buscar Pares:** Entrenamiento de memoria a corto plazo.
    * **Ordenar Secuencias:** Lógica y razonamiento.

### 🎯 Hábitos y Metas Diarias
* **Sistema de Logros:** Seguimiento de metas personalizadas como hidratación, caminatas y lectura.
* **Refuerzo Positivo:** Mensajes de motivación y recomendaciones personalizadas según el progreso alcanzado.

### 📞 Seguridad y Contacto
* **Botón de Emergencia:** Acceso rápido para contactar a un familiar o cuidador designado.
* **Directorio Simplificado:** Gestión de contactos clave con interfaz de marcación directa.

---

## 🎨 Diseño Centrado en el Usuario (A11y)

La aplicación ha sido diseñada bajo principios de **Accesibilidad Móvil**:
* **Tipografía Legible:** Uso extensivo de la fuente **Nunito** para una lectura clara.
* **Alto Contraste:** Paleta de colores cálidos y contrastados (Naranja cálido, Verde salud, Rojo emergencia).
* **Elementos Táctiles:** Botones y tarjetas de gran tamaño para facilitar la interacción con destreza motriz reducida.
* **Iconografía Intuitiva:** Apoyo visual con Emojis y Font Awesome para una navegación sin fricciones.

---

## 🛠️ Stack Tecnológico

* **Framework:** .NET 9.0 MAUI (Multi-platform App UI).
* **Arquitectura:** **MVVM (Model-View-ViewModel)** utilizando el CommunityToolkit.Mvvm para una reactividad eficiente y código limpio.
* **Persistencia:** **SQLite** (sqlite-net-pcl) para almacenamiento local robusto y funcionamiento offline.
* **Notificaciones:** Plugin.LocalNotification para alertas en tiempo real sin dependencia de servidor.
* **Inyección de Dependencias:** Uso del contenedor nativo de .NET para desacoplamiento de servicios.

---

## 🏗️ Arquitectura del Proyecto

```text
EnvejecerConBienestar/
├── Models/           # Entidades de datos (Medicamento, Meta, Contacto, etc.)
├── ViewModels/       # Lógica de presentación y binding de datos
├── Views/            # Definiciones de UI en XAML y Code-behind
├── Services/         # Servicios de negocio (Database, Alarms, Reports)
├── Helpers/          # Convertidores y constantes globales
└── Resources/        # Assets (Fuentes, Imágenes, Estilos globales)
```

---

## 🚀 Configuración y Ejecución

### Requisitos Previos
* **.NET 9 SDK**
* **Workload de MAUI** instalado (`dotnet workload install maui`)
* **IDE:** Visual Studio 2022 (v17.12+) o VS Code con la extensión de .NET MAUI.

### Instalación
1. Clonar el repositorio:
   ```bash
   git clone https://github.com/tu-usuario/EnvejecerConBienestar.git
   ```
2. Restaurar dependencias:
   ```bash
   dotnet restore
   ```
3. Ejecutar en el dispositivo/emulador:
   ```bash
   # Para Android
   dotnet build -t:Run -f net9.0-android
   ```

---

## 📝 Nota sobre Recursos (Fuentes)
La app utiliza la familia de fuentes **Nunito**. Asegúrate de que los archivos `.ttf` se encuentren en `Resources/Fonts/` para que los estilos se apliquen correctamente.

---

## 📄 Licencia
Este proyecto está bajo la Licencia MIT. Consulta el archivo `LICENSE` para más detalles.

---
*Desarrollado con ❤️ para mejorar la vida de quienes más nos cuidaron.*
