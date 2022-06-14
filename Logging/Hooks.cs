using Serilog.Sinks.File.Archive;
using System.IO.Compression;

namespace ShopOffice.Logging
{
    /// <summary>
    /// Hooks for Serilog
    /// </summary>
    public class Hooks
    {
        /// <summary>
        /// Fastest hooks
        /// </summary>
        public static ArchiveHooks FastestHooks => new ArchiveHooks(CompressionLevel.Fastest, "/logs/backups/");
    }
}
