using SQLite;
using EnvejecerConBienestar.Models;
using EnvejecerConBienestar.Helpers;

namespace EnvejecerConBienestar.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public DatabaseService()
    {
    }

    private async Task Init()
    {
        if (_database is not null)
            return;

        await _semaphore.WaitAsync();
        try
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(DatabaseConstants.DatabasePath, DatabaseConstants.Flags);

            await _database.CreateTablesAsync<Medicamento, Contacto, ActividadCognitiva>();
            await _database.CreateTablesAsync<Habito, Meta, PerfilUsuario>();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // Operaciones Genéricas
    public async Task<List<T>> GetItemsAsync<T>() where T : new()
    {
        await Init();
        return await _database!.Table<T>().ToListAsync();
    }

    public async Task<int> SaveItemAsync<T>(T item) where T : IEntity, new()
    {
        await Init();
        if (item.Id == 0)
            return await _database!.InsertAsync(item);
        else
            return await _database!.UpdateAsync(item);
    }

    public async Task<int> DeleteItemAsync<T>(T item) where T : new()
    {
        await Init();
        return await _database!.DeleteAsync(item);
    }

    // Métodos específicos
    public async Task<List<Contacto>> GetContactosAsync() => await GetItemsAsync<Contacto>();
    public async Task<Contacto> GetContactoAsync(int id) { await Init(); return await _database!.GetAsync<Contacto>(id); }
    public async Task<int> SaveContactoAsync(Contacto contacto) => await SaveItemAsync(contacto);
    
    public async Task<List<Medicamento>> GetMedicamentosAsync() => await GetItemsAsync<Medicamento>();
    public async Task<Medicamento> GetMedicamentoAsync(int id) { await Init(); return await _database!.GetAsync<Medicamento>(id); }
    public async Task<int> SaveMedicamentoAsync(Medicamento medicamento) => await SaveItemAsync(medicamento);

    public async Task<Contacto?> GetContactoEmergenciaAsync()
    {
        await Init();
        return await _database!.Table<Contacto>().Where(c => c.EsEmergencia).FirstOrDefaultAsync();
    }

    public async Task<List<Habito>> GetHabitosAsync(DateTime fecha) 
    { 
        await Init();
        var inicio = fecha.Date;
        var fin = inicio.AddDays(1);
        return await _database!.Table<Habito>()
                               .Where(h => h.Fecha >= inicio && h.Fecha < fin)
                               .ToListAsync(); 
    }
    public async Task<int> SaveHabitoAsync(Habito habito) => await SaveItemAsync(habito);

    // Métodos de Meta (nuevo sistema de metas)
    public async Task<List<Meta>> GetMetasActivasAsync()
    {
        await Init();
        var hoy = DateTime.Now.Date;
        // Incluye metas activas (pendientes) y metas completadas hoy
        return await _database!.Table<Meta>()
                               .Where(m => m.FechaFin >= hoy)
                               .ToListAsync();
    }

    public async Task<List<Meta>> GetMetasCompletadasHoyAsync()
    {
        await Init();
        var hoy = DateTime.Now.Date;
        return await _database!.Table<Meta>()
                               .Where(m => m.Completada && m.FechaFin == hoy)
                               .ToListAsync();
    }

    public async Task<List<Meta>> GetMetasPendientesAsync()
    {
        await Init();
        var hoy = DateTime.Now.Date;
        return await _database!.Table<Meta>()
                               .Where(m => !m.Completada && m.FechaFin < hoy)
                               .ToListAsync();
    }

    public async Task<Meta?> GetMetaAsync(int id)
    {
        await Init();
        return await _database!.FindAsync<Meta>(id);
    }

    public async Task<int> SaveMetaAsync(Meta meta) => await SaveItemAsync(meta);

    public async Task<int> DeleteMetaAsync(Meta meta) => await DeleteItemAsync(meta);

    // Métodos de PerfilUsuario
    public async Task<PerfilUsuario?> GetPerfilUsuarioAsync()
    {
        await Init();
        return await _database!.Table<PerfilUsuario>().FirstOrDefaultAsync();
    }

    public async Task<int> SavePerfilUsuarioAsync(PerfilUsuario perfil) => await SaveItemAsync(perfil);
}
