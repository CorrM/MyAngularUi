namespace MAU.Models;

/// <summary>
/// Struct contains request information
/// </summary>
public readonly struct RequestState
{
    /// <summary>
    /// Request Id that's used to communicate with front-end side
    /// </summary>
    public int RequestId { get; init; }

    /// <summary>
    /// Request send successfully
    /// </summary>
    public bool SuccessSent { get; init; }
}