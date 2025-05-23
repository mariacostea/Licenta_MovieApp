﻿namespace MobyLabWebProgramming.Core.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Message { get; set; } = null!;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
