using System;
using System.Collections.Concurrent;

namespace MonoGameLibrary.Core.Hosting {
    /// <summary>
    /// A thread-safe service registry that stores services by their type. 
    /// </summary>
    public class ServiceRegistry : IServiceRegistry {
        private readonly ConcurrentDictionary<Type, object> _registryService = new ConcurrentDictionary<Type, object>();
        
        /// <summary>
        /// Gets the number of registered services. 
        /// </summary>
        public int Count {
            get {
                return _registryService.Count;
            }
        }
        
        /// <inheritdoc />
        public void Register<T>(T instance) where T : class {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }

            if (!TryRegister(instance, false)) {
                throw new InvalidOperationException(
                    $"Service of type {typeof(T).FullName} is already registered. "
                );
            }
        }
        
        /// <inheritdoc />
        public bool TryRegister<T>(T instance, bool flagOverwrite = false) where T : class {
            if (instance == null) {
                throw new ArgumentNullException(nameof(instance));
            }
            
            Type type = typeof(T);
            
            if (flagOverwrite) {
                _registryService[type] = instance;
                return true;
            } else {
                return _registryService.TryAdd(type, instance);
            }
        }
        
        /// <inheritdoc />
        public T Get<T>() where T : class {
            if (TryGet<T>(out T instance)) {
                return instance;
            } else {
                throw new InvalidOperationException(
                    $"Service of type {typeof(T).FullName} is not registered. "
                );
            }
        }
        
        /// <inheritdoc />
        public bool TryGet<T>(out T instance) where T : class {
            Type type = typeof(T);

            if (_registryService.TryGetValue(type, out object rawInstance)) {
                instance = rawInstance as T;
                return instance != null;
            } else {
                instance = null;
                return false;
            }
        }
        
        /// <inheritdoc />
        public bool Contains<T>() where T : class {
            return _registryService.ContainsKey(typeof(T));
        }
        
        /// <inheritdoc />
        public bool Remove<T>() where T : class {
            return _registryService.TryRemove(typeof(T), out var valueRemoved);
        }
        
        /// <summary>
        /// Removes all registered services, returning the registry to an empty state. 
        /// </summary>
        public void Clear() {
            _registryService.Clear();
        }
    }
}