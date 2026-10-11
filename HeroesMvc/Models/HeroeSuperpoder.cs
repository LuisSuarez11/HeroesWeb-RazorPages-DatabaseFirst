namespace HeroesMvc.Models;

public class HeroeSuperpoder
{
    public int HeroeId { get; set; }
    public Heroe Heroe { get; set; } = null!;
    public int SuperpoderId { get; set; }
    public Superpoder Superpoder { get; set; } = null!;
}
