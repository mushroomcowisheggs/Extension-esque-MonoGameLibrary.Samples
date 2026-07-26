using System;

namespace MonoGameLibrary.Core.Diagnostics {
    /// <summary>
    /// A logger implementation that discards all messages. 
    /// </summary>
    public sealed class NullLogger : ILogger {
        /// <summary>
        /// Gets a shared singleton instance. 
        /// </summary>
        public static readonly NullLogger Instance = new NullLogger();
        
        private NullLogger() {
        }
        
        /// <inheritdoc />
        public void Log(LogLevel level, string message) {
        }
        
        /// <inheritdoc />
        public void Log(LogLevel level, string message, Exception exception) {
        }
        
        /// <inheritdoc />
        public bool IsEnabled(LogLevel level) {
            return false;
        }
    }
}
