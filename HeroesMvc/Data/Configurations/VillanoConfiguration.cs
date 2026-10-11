using HeroesMvc.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HeroesMvc.Data.Configurations;

public class VillanoConfiguration : IEntityTypeConfiguration<Villano>
{
    public void Configure(EntityTypeBuilder<Villano> builder)
    {
        builder.ToTable("Villanos", "dbo", t =>
            t.HasCheckConstraint(
                "CK_Villanos_NivelAmenaza",
                "[Nivel_Amenaza] BETWEEN 1 AND 5"));

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Alias)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.NivelAmenaza)
            .HasColumnName("Nivel_Amenaza");

        builder.Property(v => v.Recompensa)
            .HasPrecision(10, 2);

        builder.Property(v => v.FechaRegistro)
            .HasDefaultValueSql("GETDATE()");

        builder.HasOne(v => v.Ciudad)
            .WithMany(c => c.Villanos)
            .HasForeignKey(v => v.CiudadId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}
