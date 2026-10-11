namespace HeroesMvc.Models;

public class Vehiculo
{
    public int Id { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public decimal CostoMantenimiento { get; set; }
    public DateTime FechaAlta { get; set; }
    public int? CuartelId { get; set; }
    public Cuartel? Cuartel { get; set; }
}
