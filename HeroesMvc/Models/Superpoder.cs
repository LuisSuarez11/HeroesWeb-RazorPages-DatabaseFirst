namespace HeroesMvc.Models;

public class Superpoder
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public ICollection<HeroeSuperpoder> HeroeSuperpoderes { get; set; } = new List<HeroeSuperpoder>();
}
