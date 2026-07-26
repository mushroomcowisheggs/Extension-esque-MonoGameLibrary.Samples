using System;

namespace MonoGameLibrary.Core.Diagnostics {
    /// <summary>
    /// A simple logger that writes to the console. 
    /// </summary>
    public sealed class ConsoleLogger : ILogger {
        /// <inheritdoc />
        public void Log(LogLevel level, string message) {
            Console.WriteLine($"[{level}] {message}");
        }
        
        /// <inheritdoc />
        public void Log(LogLevel level, string message, Exception exception) {
            Console.WriteLine($"[{level}] {message} {exception}");
        }
        
        /// <inheritdoc />
        public bool IsEnabled(LogLevel level) {
            return true;
        }
    }
}
