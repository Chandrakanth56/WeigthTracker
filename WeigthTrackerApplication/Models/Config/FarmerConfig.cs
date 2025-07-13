using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace WeigthTrackerApplication.Models.Config
{
    public class FarmerConfig: IEntityTypeConfiguration<Farmer>
    {
        public void Configure(EntityTypeBuilder<Farmer> builder)
        {    
            builder.HasKey(e => e.FarmerId).HasName("PK__Farmer__731B88E806A93F94");

            builder.ToTable("Farmer");

            builder.HasIndex(e => e.FarmerEmail, "UQ__Farmer__7A7D7D0916312D57").IsUnique();

            builder.Property(e => e.FarmerId).HasColumnName("FarmerID");
            builder.Property(e => e.FarmerEmail)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            builder.Property(e => e.FarmerName)
                    .HasMaxLength(100)
                    .IsUnicode(false);
            builder.Property(e => e.PassswordHAsh)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("PassswordHAsh");
            builder.Property(e => e.VendorId).HasColumnName("VendorID");
                
            builder.HasOne(d => d.Vendor).WithMany(p => p.Farmers)
                     .HasForeignKey(d => d.VendorId)
                    .HasConstraintName("FK__Farmer__VendorID__4222D4EF");
        }

    }
    public class VendorConfig:IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
        {

            builder.HasKey(e => e.VendorId).HasName("PK__Vendor__FC8618F35D6F3EA2");

            builder.ToTable("Vendor");

            builder.HasIndex(e => e.VendorEmail, "UQ__Vendor__F0E72A771610049A").IsUnique();

            builder.Property(e => e.PasswordHash).IsUnicode(false);
            builder.Property(e => e.VendorEmail)
                .HasMaxLength(100)
                .IsUnicode(false);
            builder.Property(e => e.VendorName)
                .HasMaxLength(100)
                .IsUnicode(false);

        }
    }
    public class WeightsConfig:IEntityTypeConfiguration<Weight>
    {
        public void Configure(EntityTypeBuilder<Weight> builder)
        {
            builder.HasKey(e => e.WeightId).HasName("PK__Weights__02A0F3FB9D3AAEF5");
            builder.Property(e => e.WeightId).HasColumnName("WeightID");
            builder.Property(e => e.FarmerId).HasColumnName("FarmerID");
            builder.Property(e => e.Timestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            builder.HasOne(d => d.Farmer).WithMany(p => p.Weights)
                .HasForeignKey(d => d.FarmerId)
                .HasConstraintName("FK__Weights__FarmerI__44FF419A");

        }
    }
}
