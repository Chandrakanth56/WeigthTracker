using System;
using System.Collections.Generic;

namespace WeigthTrackerApplication.Models;

public partial class Vendor
{
    public int VendorId { get; set; }

    public string VendorName { get; set; } = null!;

    public string? VendorEmail { get; set; }

    public string? PasswordHash { get; set; }

    public virtual ICollection<Farmer> Farmers { get; set; } = new List<Farmer>();
}
