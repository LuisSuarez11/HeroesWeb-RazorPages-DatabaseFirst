namespace HeroesMvc.Models;

public class Cuartel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public int Capacidad { get; set; }
    public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
