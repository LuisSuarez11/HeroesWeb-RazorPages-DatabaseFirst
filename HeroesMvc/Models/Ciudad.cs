namespace HeroesMvc.Models;

public class Ciudad
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Pais { get; set; } = string.Empty;
    public ICollection<Villano> Villanos { get; set; } = new List<Villano>();
}
