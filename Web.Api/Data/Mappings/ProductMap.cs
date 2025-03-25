using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web.Api.Models;

namespace Web.Api.Data.Mappings;
public class ProductMap : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();
        builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnName("Name")
            .HasColumnType("VARCHAR")
            .HasMaxLength(50);
        builder.Property(x => x.Price)
            .IsRequired()
            .HasColumnName("Price")
            .HasColumnType("DECIMAL");
        builder.Property(x => x.Description)
            .IsRequired()
            .HasColumnName("Description")
            .HasColumnType("TEXT");
    }
}