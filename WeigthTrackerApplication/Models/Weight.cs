using System;
using System.Collections.Generic;

namespace WeigthTrackerApplication.Models;

public partial class Weight
{
    public int WeightId { get; set; }

    public int? FarmerId { get; set; }

    public double Weights { get; set; }

    public DateTime? Timestamp { get; set; }


    public virtual Farmer? Farmer { get; set; }
}
