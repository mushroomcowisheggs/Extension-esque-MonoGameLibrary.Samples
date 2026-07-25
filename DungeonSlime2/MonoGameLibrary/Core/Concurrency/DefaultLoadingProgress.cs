using System;
using System.Threading;

namespace MonoGameLibrary.Core.Concurrency {
    /// <summary>
    /// A simple in-memory implementation of <see cref="ILoadingProgress"/>. 
    /// </summary>
    public sealed class DefaultLoadingProgress : ILoadingProgress {
        private readonly object _gate = new object();
        private float _progressLast;
        
        /// <inheritdoc />
        public event Action<string, float> ProgressUpdated;
        
        /// <inheritdoc />
        public void Report(string nameOperation, float progress) {
            if (string.IsNullOrWhiteSpace(nameOperation)) {
                throw new ArgumentException("Operation name cannot be empty.", nameof(nameOperation));
            }
            
            if (progress < 0f || progress > 1f) {
                throw new ArgumentOutOfRangeException(nameof(progress));
            }
            
            lock (_gate) {
                _progressLast = progress;
            }
            
            var handler = ProgressUpdated;
            if (handler != null) {
                handler(nameOperation, progress);
            }
        }
        
        /// <summary>
        /// Gets the most recently reported progress value. 
        /// </summary>
        public float LastProgress {
            get {
                lock (_gate) {
                    return _progressLast;
                }
            }
        }
    }
}
