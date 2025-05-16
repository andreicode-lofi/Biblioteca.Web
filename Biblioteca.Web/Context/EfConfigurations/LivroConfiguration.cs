using Biblioteca.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace Biblioteca.Web.Context.EfConfigurations
{
    public class LivroConfiguration : IEntityTypeConfiguration<LivroModel>
    {
        public void Configure(EntityTypeBuilder<LivroModel> builder)
        {
            builder.ToTable("LivroModel");

            // Conversor JSON para TrechosFavoritos
            var trechosConverter = new ValueConverter<List<string>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );

            var trechosComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null ? v.GetHashCode() : 0)),
                c => c.ToList()
            );

            builder
                .Property(l => l.TrechosFavoritos)
                .HasConversion(trechosConverter)
                .Metadata.SetValueComparer(trechosComparer);

            builder.Property(l => l.TrechosFavoritos).HasColumnType("jsonb");
        }
    }
}
