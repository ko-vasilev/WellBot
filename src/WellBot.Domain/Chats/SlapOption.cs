﻿using System.ComponentModel.DataAnnotations;

namespace WellBot.Domain.Chats;

/// <summary>
/// Possible slap option.
/// </summary>
public class SlapOption
{
    /// <summary>
    /// Option id.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Animation file id.
    /// </summary>
    [MaxLength(150)]
    public required string FileId { get; set; }
}
