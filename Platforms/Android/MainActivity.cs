using Android.App;
using Android.Content.PM;
using Android.OS;

// ─────────────────────────────────────────────────────────────
// PERMISOS declarados aquí como atributos de ensamblado.
// Con Android SDK 35.0.105 en .NET 9, esta es la única forma
// de que el merger los incluya CON el atributo android:name
// correcto y no produzca el error AMM0000.
// ─────────────────────────────────────────────────────────────
[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]
[assembly: UsesPermission(Android.Manifest.Permission.Vibrate)]
[assembly: UsesPermission("android.permission.POST_NOTIFICATIONS")]

namespace EnvejecerConBienestar;

[Activity(
    // El tema debe existir en Resources/values/styles.xml (MAUI lo genera automáticamente)
    Theme = "@style/Maui.MainTheme.NoActionBar",

    // MainLauncher = true → esta Activity es el punto de entrada de la app
    MainLauncher = true,

    // ConfigurationChanges evita que la Activity se destruya y recree
    // en cada rotación o cambio de tema — crítico para MAUI
    ConfigurationChanges =
        ConfigChanges.ScreenSize |
        ConfigChanges.Orientation |
        ConfigChanges.UiMode |
        ConfigChanges.ScreenLayout |
        ConfigChanges.SmallestScreenSize |
        ConfigChanges.Density |
        ConfigChanges.Keyboard |
        ConfigChanges.KeyboardHidden |
        ConfigChanges.Navigation
)]
public class MainActivity : MauiAppCompatActivity
{
    // MauiAppCompatActivity maneja todo el ciclo de vida.
    // No se necesita override de OnCreate salvo para configuración
    // específica de Android (ej. permisos en runtime).
}
