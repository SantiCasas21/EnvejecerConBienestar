using System.Text;
using EnvejecerConBienestar.Models;

namespace EnvejecerConBienestar.Services;

public class ReportService
{
    private readonly DatabaseService _databaseService;

    public ReportService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<string> GenerarReporteMedicoAsync()
    {
        var perfil = await _databaseService.GetPerfilUsuarioAsync();
        var medicamentos = await _databaseService.GetMedicamentosAsync();
        var contactos = await _databaseService.GetContactosAsync();

        var sb = new StringBuilder();

        // ═══════════ HEADER ═══════════
        sb.AppendLine("═══════════════════════════════════════════════");
        sb.AppendLine("       INFORME MEDICO PERSONAL");
        sb.AppendLine("   Envejecer Con Bienestar");
        sb.AppendLine("═══════════════════════════════════════════════");
        sb.AppendLine($"Fecha: {DateTime.Now:dd/MM/yyyy}");
        sb.AppendLine($"Hora:  {DateTime.Now:HH:mm}");
        sb.AppendLine();

        // ═══════════ DATOS DEL PACIENTE ═══════════
        sb.AppendLine("───────────────────────────────────────────────");
        sb.AppendLine("  DATOS DEL PACIENTE");
        sb.AppendLine("───────────────────────────────────────────────");
        if (perfil != null && perfil.EstaCompleto)
        {
            sb.AppendLine($"  Nombre:          {perfil.Nombre}");
            sb.AppendLine($"  Edad:            {perfil.TextoEdad}");
            sb.AppendLine($"  Tipo de Sangre:  {perfil.TextoTipoSangre}");
            sb.AppendLine($"  Telefono:        {perfil.Telefono}");
            sb.AppendLine($"  Alergias:        {perfil.TextoAlergias}");
            sb.AppendLine($"  Condiciones:     {perfil.TextoCondiciones}");
        }
        else
        {
            var nombre = Preferences.Get("nombre_usuario", "No registrado");
            sb.AppendLine($"  Nombre:          {nombre}");
            sb.AppendLine("  (Complete su perfil medico en la app para mas detalles)");
        }
        sb.AppendLine();

        // ═══════════ MEDICAMENTOS ═══════════
        sb.AppendLine("───────────────────────────────────────────────");
        sb.AppendLine("  MEDICAMENTOS ACTUALES");
        sb.AppendLine("───────────────────────────────────────────────");

        if (medicamentos.Any())
        {
            sb.AppendLine($"  Total de medicamentos registrados: {medicamentos.Count}");
            sb.AppendLine();

            foreach (var med in medicamentos)
            {
                var estado = med.EstaTomado ? "[TOMADO]" : "[PENDIENTE]";
                sb.AppendLine($"  {med.Icono} {med.Nombre} - {med.Miligramos} mg");
                sb.AppendLine($"     Dosis: Cada {med.Frecuencia} horas");
                sb.AppendLine($"     Hora de inicio: {med.HoraAlarma:hh\\:mm}");
                if (!string.IsNullOrWhiteSpace(med.Notas))
                    sb.AppendLine($"     Notas: {med.Notas}");
                sb.AppendLine($"     Tomas por dia: {med.TomasPorDia}");
                if (med.CantidadRestante > 0)
                    sb.AppendLine($"     Pastillas restantes: {med.CantidadRestante}");
                sb.AppendLine($"     Estado hoy: {estado}");
                sb.AppendLine();
            }

            var tomados = medicamentos.Count(m => m.EstaTomado);
            var porcentaje = medicamentos.Count > 0 ? (double)tomados / medicamentos.Count * 100 : 0;
            sb.AppendLine("───────────────────────────────────────────────");
            sb.AppendLine("  RESUMEN DE CUMPLIMIENTO");
            sb.AppendLine("───────────────────────────────────────────────");
            sb.AppendLine($"  Medicinas tomadas hoy:  {tomados} de {medicamentos.Count}");
            sb.AppendLine($"  Porcentaje:             {porcentaje:F0}%");
            sb.AppendLine();
        }
        else
        {
            sb.AppendLine("  No hay medicamentos registrados.");
            sb.AppendLine();
        }

        // ═══════════ CONTACTOS DE EMERGENCIA ═══════════
        sb.AppendLine("───────────────────────────────────────────────");
        sb.AppendLine("  CONTACTOS DE EMERGENCIA");
        sb.AppendLine("───────────────────────────────────────────────");

        var emergencias = contactos.Where(c => c.EsEmergencia).ToList();
        if (emergencias.Any())
        {
            foreach (var c in emergencias)
            {
                sb.AppendLine($"  {c.Inicial} {c.Nombre}");
                sb.AppendLine($"     Relacion: {c.Relacion}");
                sb.AppendLine($"     Telefono: {c.Telefono}");
                if (!string.IsNullOrWhiteSpace(c.Ubicacion))
                    sb.AppendLine($"     Direccion: {c.Ubicacion}");
                sb.AppendLine();
            }
        }
        else
        {
            sb.AppendLine("  No hay contactos de emergencia registrados.");
            sb.AppendLine("  Configure un contacto SOS en la seccion Contactos.");
            sb.AppendLine();
        }

        // ═══════════ FOOTER ═══════════
        sb.AppendLine("═══════════════════════════════════════════════");
        sb.AppendLine("  Generado por Envejecer Con Bienestar");
        sb.AppendLine("  Aplicacion de apoyo para adultos mayores");
        sb.AppendLine("═══════════════════════════════════════════════");

        return sb.ToString();
    }

    public async Task CompartirReporteAsync()
    {
        var reporte = await GenerarReporteMedicoAsync();
        var titulo = $"Reporte_Medico_{DateTime.Now:yyyyMMdd_HHmm}.txt";

        var ruta = Path.Combine(FileSystem.CacheDirectory, titulo);
        await File.WriteAllTextAsync(ruta, reporte);

        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Compartir Reporte Medico",
            File = new ShareFile(ruta)
        });
    }
}
