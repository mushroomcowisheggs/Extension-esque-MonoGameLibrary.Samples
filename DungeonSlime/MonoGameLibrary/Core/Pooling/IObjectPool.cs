using System;

namespace MonoGameLibrary.Core.Pooling {
    /// <summary>
    /// A pool of reusable objects.
    /// </summary>
    /// <typeparam name="T">The type of objects in the pool.</typeparam>
    public interface IObjectPool<T> where T : class {
        /// <summary>
        /// Retrieves an object from the pool. If the pool is empty, a new instance is created.
        /// </summary>
        /// <returns>A pooled or newly created object.</returns>
        T Get();

        /// <summary>
        /// Attempts to retrieve an object from the pool without creating a new one if empty.
        /// </summary>
        /// <param name="item">When this method returns, contains the retrieved object if successful; otherwise, <c>null</c>.</param>
        /// <returns><c>true</c> if an object was available; otherwise <c>false</c>.</returns>
        bool TryGet(out T item);

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        /// <param name="item">The object to return.</param>
        void Return(T item);

        /// <summary>
        /// Gets the number of objects currently in use (taken from the pool).
        /// </summary>
        int CountInUse { get; }

        /// <summary>
        /// Gets the number of objects available in the pool.
        /// </summary>
        int CountAvailable { get; }

        /// <summary>
        /// Clears all objects from the pool.
        /// </summary>
        void Clear();
    }
}