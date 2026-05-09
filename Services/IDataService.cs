using EnvejecerConBienestar.Models;

namespace EnvejecerConBienestar.Services;

public interface IDataService
{
    Task<IEnumerable<Contacto>> GetContactosAsync();
    Task AddContactoAsync(Contacto contacto);
}
