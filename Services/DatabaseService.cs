using SQLite;
using EnvejecerConBienestar.Models;
using EnvejecerConBienestar.Helpers;

namespace EnvejecerConBienestar.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    public DatabaseService()
    {
    }

    private async Task Init()
    {
        if (_database is not null)
            return;

        _database = new SQLiteAsyncConnection(DatabaseConstants.DatabasePath, DatabaseConstants.Flags);

        await _database.CreateTableAsync<Medicamento>();
        await _database.CreateTableAsync<Contacto>();
        await _database.CreateTableAsync<ActividadCognitiva>();
        await _database.CreateTableAsync<Habito>();
    }

    // Operaciones Genéricas
    public async Task<List<T>> GetItemsAsync<T>() where T : new()
    {
        await Init();
        return await _database!.Table<T>().ToListAsync();
    }

    public async Task<int> SaveItemAsync<T>(T item) where T : new()
    {
        await Init();
        var props = typeof(T).GetProperties();
        var idProp = props.FirstOrDefault(p => p.Name == "Id");

        if (idProp != null)
        {
            var idValue = (int)idProp.GetValue(item)!;
            if (idValue != 0)
            {
                return await _database!.UpdateAsync(item);
            }
        }
        return await _database!.InsertAsync(item);
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
}
