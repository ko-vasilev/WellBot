namespace WellBot.Web.Infrastructure.Middlewares;

/// <summary>
/// Problem field DTO.
/// </summary>
internal class ProblemFieldDto
{
    /// <summary>
    /// Field name.
    /// </summary>
    public string Field { get; }

    /// <summary>
    /// Field error detail.
    /// </summary>
    public string Detail { get; }

    /// <summary>
    /// Problem field with no messages.
    /// </summary>
    public static ProblemFieldDto Empty { get; } = new ProblemFieldDto(string.Empty, string.Empty);

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="field">Field name.</param>
    /// <param name="detail">Field error detail.</param>
    public ProblemFieldDto(string field, string detail)
    {
        Field = field;
        Detail = detail;
    }
}
