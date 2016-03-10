using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace EffinghamLibrary.Helpers
{
    public class ThreadedPipeStream : AbstractStreamBase
    {
        #region Fields and Properties

        private Stream threadedStream;

        private readonly BlockingCollection<byte[]> bytePipe;

        private readonly Task processingTask;

        public override bool CanWrite => true;
        #endregion Fields and Properties

        #region Constructors
        public ThreadedPipeStream(Stream threadedStream)
        {
            this.threadedStream = threadedStream;
            bytePipe = new BlockingCollection<byte[]>();
            processingTask = Task.Factory.StartNew(
                () =>
                {
                    foreach (byte[] chunk in bytePipe.GetConsumingEnumerable()) // Will try to read from blocking collection until there's data to read.
                    {
                        threadedStream.Write(chunk, 0, chunk.Length); // Write to the stream while data is being fed into it.
                    }
                }, TaskCreationOptions.LongRunning);
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream. Instead of calling this method, ensure that the stream is properly disposed.
        /// </summary>
        public override void Close()
        {
            bytePipe.CompleteAdding(); // Says to the blocking collection that we're done adding data to it.

            try
            {
                processingTask.Wait(); // Wait until the data has been read out of the stream before continuing with the close operation.
            }
            finally
            {
                base.Close();
            }
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count"/> bytes from <paramref name="buffer"/> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer"/> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] chunk = new byte[count];
            Buffer.BlockCopy(buffer, offset, chunk, 0, count);
            bytePipe.Add(chunk);
        }
        #endregion Methods
    }
}
