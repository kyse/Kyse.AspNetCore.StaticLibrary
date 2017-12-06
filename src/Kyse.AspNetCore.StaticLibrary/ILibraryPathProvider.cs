//Created by Jared Fisher (kyse@kyse.us)

using System.Collections.Generic;

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// The interface for the class providing the ILibrary list.
    /// </summary>
    public interface ILibraryPathProvider
    {
        /// <summary>
        /// Returns the the list of libraries exposed by this static library server.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ILibrary> GetLibraries();
    }
}
