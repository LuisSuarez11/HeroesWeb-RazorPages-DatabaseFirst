namespace HeroesMvc.Models;

public class Villano
{
    public int Id { get; set; }
    public string Alias { get; set; } = string.Empty;
    public int NivelAmenaza { get; set; }
    public decimal Recompensa { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int CiudadId { get; set; }
    public Ciudad Ciudad { get; set; } = null!;
}
