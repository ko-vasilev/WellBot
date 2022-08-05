using System.IO;

namespace WellBot.Infrastructure
{
    /// <summary>
    /// A file stream that automatically deletes a file after it's been disposed.
    /// </summary>
    internal class AutoCleanupFileStream : Stream
    {
        private readonly FileStream fileStream;
        private readonly string filePath;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filePath">Path to the file.</param>
        public AutoCleanupFileStream(string filePath)
        {
            this.filePath = filePath;
            fileStream = File.OpenRead(filePath);
        }

        /// <inheritdoc/>
        public override bool CanRead => fileStream.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => fileStream.CanSeek;

        /// <inheritdoc/>
        public override bool CanWrite => fileStream.CanWrite;

        /// <inheritdoc/>
        public override long Length => fileStream.Length;

        /// <inheritdoc/>
        public override long Position
        {
            get => fileStream.Position;
            set => fileStream.Position = value;
        }

        /// <inheritdoc/>
        public override void Flush() => fileStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => fileStream.Read(buffer, offset, count);
        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin) => fileStream.Seek(offset, origin);
        public override void SetLength(long value) => fileStream.SetLength(value);
        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count) => fileStream.Write(buffer, offset, count);

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                fileStream.Dispose();
                File.Delete(filePath);
            }
        }
    }
}
