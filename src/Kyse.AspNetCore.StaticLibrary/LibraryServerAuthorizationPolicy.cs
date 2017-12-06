//Created by Jared Fisher (kyse@kyse.us)

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// Constant strings for the authorization policy names.
    /// </summary>
    public static class LibraryServerAuthorizationPolicy
    {
        /// <summary>
        /// LibraryFileMiddleware Authorization Policy Name
        /// </summary>
        public static readonly string File = "StaticLibraryFile";
        /// <summary>
        /// LibraryBrowserMiddleware Authorization Policy Name
        /// </summary>
        public static readonly string Browser = "StaticLibraryBrowser";
    }
}
