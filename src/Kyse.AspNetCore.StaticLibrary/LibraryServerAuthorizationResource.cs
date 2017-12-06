//Created by Jared Fisher (kyse@kyse.us)

using Microsoft.AspNetCore.Http;

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// StaticLibrary Resource class to wrap values for authorization reuqirement handlers.
    /// </summary>
    public class LibraryServerAuthorizationResource
    {
        /// <summary>
        /// StaticLibrary Resource class to wrap values for authorization reuqirement handlers.
        /// </summary>
        public LibraryServerAuthorizationResource() { }

        /// <summary>
        /// StaticLibrary Resource class to wrap values for authorization reuqirement handlers.
        /// </summary>
        /// <param name="httpContext">The request HttpContext</param>
        /// <param name="library">The ILibrary that matches the request</param>
        public LibraryServerAuthorizationResource(HttpContext httpContext, ILibrary library)
        {
            HttpContext = httpContext;
            Library = library;
        }

        /// <summary>
        /// The request HttpContext
        /// </summary>
        public HttpContext HttpContext { get; set; }
        /// <summary>
        /// The ILibrary that matches the request
        /// </summary>
        public ILibrary Library { get; set; }
    }
}
