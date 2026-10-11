using HeroesMvc.Data;
using HeroesMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeroesMvc.Controllers;

public class LaboratorioEfController : Controller
{
    private readonly HeroesDbContext _context;

    public LaboratorioEfController(HeroesDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Relaciones(int id)
    {
        var heroe = await _context.Heroes
            .Include(h => h.Equipo)
            .Include(h => h.Ficha)
            .Include(h => h.HeroeSuperpoderes).ThenInclude(hs => hs.Superpoder)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (heroe is null)
            return NotFound();

        return Json(new
        {
            heroe.Id,
            heroe.Nombre,
            heroe.Ciudad,
            Equipo = heroe.Equipo?.Nombre,
            IdentidadSecreta = heroe.Ficha?.IdentidadSecreta,
            Debilidad = heroe.Ficha?.Debilidad,
            Superpoderes = heroe.HeroeSuperpoderes.Select(hs => hs.Superpoder.Nombre).ToList()
        });
    }

    [HttpGet]
    public async Task<IActionResult> Ciudad(int id)
    {
        var ciudad = await _context.Ciudades
            .Include(c => c.Villanos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (ciudad is null)
            return NotFound();

        return Json(new
        {
            ciudad.Id,
            ciudad.Nombre,
            ciudad.Pais,
            Villanos = ciudad.Villanos.Select(v => new
            {
                v.Alias,
                v.NivelAmenaza,
                v.Recompensa,
                v.FechaRegistro
            }).ToList()
        });
    }

    [HttpGet]
    public IActionResult ModeloVillano()
    {
        var entidad = _context.Model.FindEntityType(typeof(Villano))!;

        return Json(new
        {
            Tabla = entidad.GetTableName(),
            Esquema = entidad.GetSchema(),
            Columnas = entidad.GetProperties().Select(p => new
            {
                Propiedad = p.Name,
                Columna = p.GetColumnName(),
                Obligatoria = !p.IsNullable,
                Longitud = p.GetMaxLength(),
                Precision = p.GetPrecision(),
                Escala = p.GetScale(),
                DefaultSql = p.GetDefaultValueSql()
            })
        });
    }

    [HttpGet]
    public async Task<IActionResult> Cuartel(int id)
    {
        var cuartel = await _context.Cuarteles
            .Include(c => c.Vehiculos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cuartel is null)
            return NotFound();

        return Json(new
        {
            cuartel.Id,
            cuartel.Nombre,
            cuartel.Capacidad,
            Vehiculos = cuartel.Vehiculos.Select(v => new
            {
                v.Placa,
                v.Modelo,
                v.CostoMantenimiento,
                v.FechaAlta
            }).ToList()
        });
    }
}
