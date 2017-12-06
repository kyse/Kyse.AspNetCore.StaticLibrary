//Created by Jared Fisher (kyse@kyse.us)

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// Interface for class providing library info
    /// </summary>
    public interface ILibrary
    {
        /// <summary>
        /// The library name used for the sub path route.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The path local host path to expose with this library.
        /// </summary>
        string Path { get; set; }
    }
}