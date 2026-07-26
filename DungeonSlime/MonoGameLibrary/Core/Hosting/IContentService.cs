namespace MonoGameLibrary.Core.Hosting {
    /// <summary>
    /// Provides content loading capabilities. 
    /// </summary>
    public interface IContentService : System.IDisposable {
        /// <summary>
        /// Loads an asset of the specified type. 
        /// </summary>
        /// <typeparam name="T">The type of the asset. </typeparam>
        /// <param name="nameAsset">The asset name relative to the content root. </param>
        /// <returns>The loaded asset.</returns>
        T Load<T>(string nameAsset);

        /// <summary>
        /// Unloads all content managed by this service. 
        /// </summary>
        void Unload();
    }
}