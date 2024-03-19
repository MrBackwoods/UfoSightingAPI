using System;
using System.Collections.Generic;

namespace UfoSightingAPI.Models;

public partial class Sighting
{
    public int SightingId { get; set; }

    public int ReportedBy { get; set; }

    public DateTime? Reported { get; set; }

    public DateTime Occurred { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public int? EstimatedDurationInSeconds { get; set; }

    public string Description { get; set; } = null!;

    public int WitnessCount { get; set; }
}
