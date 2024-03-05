namespace WellBot.Web.Infrastructure.Dtos;

/// <summary>
/// DTO for application version.
/// </summary>
public class AppVersionDto
{
    /// <summary>
    /// Information version.
    /// </summary>
    public string? InformationalVersion { get; set; }

    /// <summary>
    /// Assembly version.
    /// </summary>
    public string? AssemblyVersion { get; set; }
}
