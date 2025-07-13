using System;
using System.Collections.Generic;

namespace WeigthTrackerApplication.Models;

public partial class Farmer
{
    public int FarmerId { get; set; }

    public int? VendorId { get; set; }

    public string FarmerName { get; set; } = null!;

    public string? FarmerEmail { get; set; }

    public string? PassswordHAsh { get; set; }

    public virtual Vendor? Vendor { get; set; }

    public virtual ICollection<Weight> Weights { get; set; } = new List<Weight>();
}
