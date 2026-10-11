using HeroesMvc.Data.Configurations;
using HeroesMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace HeroesMvc.Data;

public class HeroesDbContext : DbContext
{
    public HeroesDbContext(DbContextOptions<HeroesDbContext> options)
        : base(options)
    {
    }

    // Clase 2
    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<Heroe> Heroes => Set<Heroe>();
    public DbSet<Ficha> Fichas => Set<Ficha>();
    public DbSet<Superpoder> Superpoderes => Set<Superpoder>();
    public DbSet<HeroeSuperpoder> HeroeSuperpoderes => Set<HeroeSuperpoder>();

    // Clase 02 (ejemplo)
    public DbSet<Ciudad> Ciudades => Set<Ciudad>();
    public DbSet<Villano> Villanos => Set<Villano>();

    // Clase 02 (ejercicio)
    public DbSet<Cuartel> Cuarteles => Set<Cuartel>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---- Mapeo de la clase 2 (se conserva en el contexto) ----
        modelBuilder.Entity<Equipo>(e =>
        {
            e.ToTable("Equipos", "dbo");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
            e.HasIndex(x => x.Nombre).IsUnique().HasDatabaseName("UQ_Equipos_Nombre");
        });

        modelBuilder.Entity<Heroe>(e =>
        {
            e.ToTable("Heroes", "dbo");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
            e.Property(x => x.Ciudad).IsRequired().HasMaxLength(100);
            e.HasOne(x => x.Equipo)
                .WithMany(q => q.Heroes)
                .HasForeignKey(x => x.EquipoId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Ficha>(e =>
        {
            e.ToTable("Fichas", "dbo");
            e.HasKey(x => x.HeroeId);
            e.Property(x => x.IdentidadSecreta).HasMaxLength(100);
            e.Property(x => x.Debilidad).HasMaxLength(150);
            e.HasOne(x => x.Heroe)
                .WithOne(h => h.Ficha)
                .HasForeignKey<Ficha>(x => x.HeroeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Superpoder>(e =>
        {
            e.ToTable("Superpoderes", "dbo");
            e.HasKey(x => x.Id);
            e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
            e.Property(x => x.Descripcion).HasMaxLength(250);
        });

        modelBuilder.Entity<HeroeSuperpoder>(e =>
        {
            e.ToTable("HeroeSuperpoder", "dbo");
            e.HasKey(x => new { x.HeroeId, x.SuperpoderId });
            e.HasOne(x => x.Heroe)
                .WithMany(h => h.HeroeSuperpoderes)
                .HasForeignKey(x => x.HeroeId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Superpoder)
                .WithMany(s => s.HeroeSuperpoderes)
                .HasForeignKey(x => x.SuperpoderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ---- Clase 02: archivos de configuracion ----
        modelBuilder.ApplyConfiguration(new CiudadConfiguration());
        modelBuilder.ApplyConfiguration(new VillanoConfiguration());

        // ---- Ejercicio Cuartel-Vehiculo ----
        modelBuilder.ApplyConfiguration(new CuartelConfiguration());
        modelBuilder.ApplyConfiguration(new VehiculoConfiguration());
    }
}
