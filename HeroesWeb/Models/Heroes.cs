using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HeroesWeb.Models;

public partial class Heroes
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string Nombre { get; set; } = null!;

    [StringLength(100)]
    public string Ciudad { get; set; } = null!;

    [StringLength(100)]
    public string? IdentidadSecreta { get; set; }

    [InverseProperty("Heroe")]
    public virtual ICollection<SuperPoderes> SuperPoderes { get; set; } = new List<SuperPoderes>();
}
