using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Kyse.AspNetCore.StaticLibrary
{
    public class LibraryServerAuthorizationResource
    {
        public LibraryServerAuthorizationResource() { }

        public LibraryServerAuthorizationResource(HttpContext httpContext, ILibrary library)
        {
            HttpContext = httpContext;
            Library = library;
        }

        public HttpContext HttpContext { get; set; }
        public ILibrary Library { get; set; }
    }
}
