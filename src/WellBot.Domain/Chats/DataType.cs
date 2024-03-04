namespace WellBot.Domain.Chats
{
    /// <summary>
    /// Type of data.
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// A photo.
        /// </summary>
        Photo = 0,

        /// <summary>
        /// Audio file.
        /// </summary>
        Audio = 1,

        /// <summary>
        /// An abstract document.
        /// </summary>
        Document = 2,

        /// <summary>
        /// An animation.
        /// </summary>
        Animation = 3,

        /// <summary>
        /// A sticker.
        /// </summary>
        Sticker = 4,

        /// <summary>
        /// A video file.
        /// </summary>
        Video = 5,

        /// <summary>
        /// A video note.
        /// </summary>
        VideoNote = 6,

        /// <summary>
        /// A recorded voice message.
        /// </summary>
        Voice = 7,

        /// <summary>
        /// Simple text.
        /// </summary>
        Text = 8,
    }
}
