namespace WellBot.Domain.Chats
{
    /// <summary>
    /// Weight of a single message to pick random message.
    /// </summary>
    public enum MessageWeight
    {
        /// <summary>
        /// Normal weight.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// High priority.
        /// </summary>
        High = 1,

        /// <summary>
        /// Highest priority.
        /// </summary>
        Highest = 2
    }
}
