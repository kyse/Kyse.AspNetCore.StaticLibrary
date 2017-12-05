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
using Microsoft.Extensions.Options;

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// Extension methods for the DirectoryBrowserMiddleware
    /// </summary>
    public static class LibraryBrowserExtensions
    {
        /// <summary>
        /// Enable directory browsing on the current path
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLibraryBrowser(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<LibraryBrowserMiddleware>();
        }

        /// <summary>
        /// Enables directory browsing for the given request path
        /// </summary>
        /// <param name="app"></param>
        /// <param name="requestPath">The relative request path.</param>
        /// <returns></returns>
        public static IApplicationBuilder UseLibraryBrowser(this IApplicationBuilder app, string requestPath)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                RequestPath = new PathString(requestPath)
            });
        }

        /// <summary>
        /// Enable directory browsing with the given options
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseLibraryBrowser(this IApplicationBuilder app, LibraryBrowserOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            return app.UseMiddleware<LibraryBrowserMiddleware>(Options.Create(options));
        }
    }
}