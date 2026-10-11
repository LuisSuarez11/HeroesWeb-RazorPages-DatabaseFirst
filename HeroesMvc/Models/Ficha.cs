namespace HeroesMvc.Models;

public class Ficha
{
    public int HeroeId { get; set; }
    public string? IdentidadSecreta { get; set; }
    public string? Debilidad { get; set; }
    public Heroe Heroe { get; set; } = null!;
}
