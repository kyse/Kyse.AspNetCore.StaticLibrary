//Created by Jared Fisher (kyse@kyse.us)

using System.Collections.Generic;

namespace Kyse.AspNetCore.StaticLibrary
{
    public interface ILibraryPathProvider
    {
        IEnumerable<ILibrary> GetLibraries();
    }
}
