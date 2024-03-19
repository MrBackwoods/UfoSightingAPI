using System;
using System.Collections.Generic;

namespace UfoSightingAPI.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly? JoinDate { get; set; }

    public string? ApiKey { get; set; }

    public DateOnly? ApiKeyActivationDate { get; set; }

    public DateOnly? ApiKeyDeactivationDate { get; set; }

    public bool? IsAdmin { get; set; }
}
