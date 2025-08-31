
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using WeigthTrackerApplication.Models.Config;

namespace WeigthTrackerApplication.Models;

public partial class WightListContext : DbContext
{
    public WightListContext()
    {
    }

    public WightListContext(DbContextOptions<WightListContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Farmer> Farmers { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    public virtual DbSet<Weight> Weights { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       if(!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=DELL\\SQLEXPRESS;Initial Catalog=WightList;Integrated Security=True;Encrypt=False");
        }
    }
       

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       OnModelCreatingPartial(modelBuilder);
    }

    protected void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FarmerConfig());
        modelBuilder.ApplyConfiguration(new VendorConfig());
        modelBuilder.ApplyConfiguration(new WeightsConfig());
    }
}
