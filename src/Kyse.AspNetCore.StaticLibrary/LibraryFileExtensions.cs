// Modified by: Jared Fisher (kyse@kyse.us)
// Changes:
// - Updated to support class name changes.
//
// Original Copywrite/License Notice:
//
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// Extension methods for the StaticFileMiddleware
    /// </summary>
    public static class LibraryFileExtensions
    {
        /// <summary>
        /// Enables static file serving for the current request path
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLibraryFiles(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            
            return app.UseMiddleware<LibraryFileMiddleware>();
        }

        /// <summary>
        /// Enables static file serving for the given request path
        /// </summary>
        /// <param name="app"></param>
        /// <param name="requestPath">The relative request path.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseLibraryFiles(this IApplicationBuilder app, string requestPath)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseLibraryFiles(new LibraryFileOptions
            {
                RequestPath = new PathString(requestPath)
            });
        }

        /// <summary>
        /// Enables static file serving with the given options
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLibraryFiles(this IApplicationBuilder app, LibraryFileOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            return app.UseMiddleware<LibraryFileMiddleware>(Options.Create(options));
        }
    }
}