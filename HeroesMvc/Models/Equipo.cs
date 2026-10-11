namespace HeroesMvc.Models;

public class Equipo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public ICollection<Heroe> Heroes { get; set; } = new List<Heroe>();
}
