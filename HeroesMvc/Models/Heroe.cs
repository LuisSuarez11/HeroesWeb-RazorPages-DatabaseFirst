namespace HeroesMvc.Models;

public class Heroe
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Ciudad { get; set; } = string.Empty;
    public int? EquipoId { get; set; }
    public Equipo? Equipo { get; set; }
    public Ficha? Ficha { get; set; }
    public ICollection<HeroeSuperpoder> HeroeSuperpoderes { get; set; } = new List<HeroeSuperpoder>();
}
