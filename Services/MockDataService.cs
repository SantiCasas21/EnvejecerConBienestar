using EnvejecerConBienestar.Models;

namespace EnvejecerConBienestar.Services;

public class MockDataService : IDataService
{
    private readonly List<Contacto> _contactos;

    public MockDataService()
    {
        _contactos = new List<Contacto>
        {
            new Contacto { Id = 1, Nombre = "Línea de Emergencias", Telefono = "123", Categoria = "SOS", Icono = "🚨" },
            new Contacto { Id = 2, Nombre = "Policía Bogotá", Telefono = "112", Categoria = "SOS", Icono = "👮" },
            new Contacto { Id = 3, Nombre = "Carmen López (Hija)", Telefono = "3101234567", Categoria = "Familia/Amigos", Icono = "👧" },
            new Contacto { Id = 4, Nombre = "Dr. Martínez (Médico)", Telefono = "3007654321", Categoria = "Familia/Amigos", Icono = "👨‍⚕️" },
            new Contacto { Id = 5, Nombre = "Luis Pérez (Vecino)", Telefono = "3209876543", Categoria = "Familia/Amigos", Icono = "🏠" }
        };
    }

    public Task<IEnumerable<Contacto>> GetContactosAsync()
    {
        return Task.FromResult(_contactos.AsEnumerable());
    }

    public Task AddContactoAsync(Contacto contacto)
    {
        contacto.Id = (_contactos.Count > 0 ? _contactos.Max(c => c.Id) : 0) + 1;
        _contactos.Add(contacto);
        return Task.CompletedTask;
    }
}