// Modified by: Jared Fisher (kyse@kyse.us)
// Changes:
// - Updated to support class name changes.
// - Added authentication and authorization.
// - Added dynamic library path mapping.
// - Converted Invoke to async
//
// Original Copywrite/License Notice:
//
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// Enables library browsing
    /// </summary>
    public class LibraryBrowserMiddleware
    {
        private readonly LibraryBrowserOptions _options;
        private readonly PathString _prefixUrl;
        private readonly RequestDelegate _next;
        private readonly IDirectoryFormatter _formatter;
        private readonly IAuthorizationService _authService;
        private readonly ILibraryPathProvider _pathProvider;

        /// <summary>
        /// Creates a new instance of the LibraryBrowserMiddleware. Using <see cref="HtmlEncoder.Default"/> instance.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="hostingEnv">The <see cref="IHostingEnvironment"/> used by this middleware.</param>
        /// <param name="authService">The authorization service used to authorize the request.</param>
        /// <param name="pathProvider">The ILibraryPathProvider service used to determine paths to serve.</param>
        /// <param name="options">The configuration for this middleware.</param>
        public LibraryBrowserMiddleware(RequestDelegate next, IHostingEnvironment hostingEnv, IAuthorizationService authService, ILibraryPathProvider pathProvider, IOptions<LibraryBrowserOptions> options)
            : this(next, hostingEnv, authService, pathProvider, HtmlEncoder.Default, options)
        {
        }

        /// <summary>
        /// Creates a new instance of the SendFileMiddleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="hostingEnv">The <see cref="IHostingEnvironment"/> used by this middleware.</param>
        /// <param name="authService">The authorization service used to authorize the request.</param>
        /// <param name="pathProvider">The ILibraryPathProvider service used to determine paths to serve.</param>
        /// <param name="encoder">The <see cref="HtmlEncoder"/> used by the default <see cref="HtmlDirectoryFormatter"/>.</param>
        /// <param name="options">The configuration for this middleware.</param>
        public LibraryBrowserMiddleware(RequestDelegate next, IHostingEnvironment hostingEnv, IAuthorizationService authService, ILibraryPathProvider pathProvider, HtmlEncoder encoder, IOptions<LibraryBrowserOptions> options)
        {
            if (hostingEnv == null)
            {
                throw new ArgumentNullException(nameof(hostingEnv));
            }

            if (encoder == null)
            {
                throw new ArgumentNullException(nameof(encoder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options.Value;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
            _formatter = options.Value.Formatter ?? new LibraryBrowserFormtter(encoder);
            _prefixUrl = _options.RequestPath;
        }

        /// <summary>
        /// Examines the request to see if it matches a configured directory.  If so, a view of the directory contents is returned.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            // Check if the URL matches any expected paths
            if (!Helpers.IsGetOrHeadMethod(context.Request.Method) ||
                !Helpers.TryMatchPath(context, _prefixUrl, forDirectory: true, subpath: out PathString libraryPath) ||
                !Helpers.TryMatchLibrary(libraryPath, _pathProvider.GetLibraries(), forDirectory: true,
                                         subpath: out PathString subpath, library: out ILibrary library))
            {
                await _next(context);
                return;
            }

            if (!context.IsAuthenticated(_options))
            {
                context.Response.StatusCode = 401;
                return;
            }

            if (!context.IsAuthorized(_options, library, LibraryServerAuthorizationPolicy.Browser))
            {
                context.Response.StatusCode = 403;
                return;
            }

            // If the path matches a directory but does not end in a slash, redirect to add the slash.
            // This prevents relative links from breaking.
            if (!Helpers.PathEndsInSlash(context.Request.Path))
            {
                context.Response.StatusCode = 301;
                context.Response.Headers[HeaderNames.Location] =
                    context.Request.PathBase + context.Request.Path + "/" + context.Request.QueryString;
                return;
            }

            if (TryGetDirectoryInfo(library, subpath, out IDirectoryContents contents))
            {
                await _formatter.GenerateContentAsync(context, contents);
                return;
            }

            context.Response.StatusCode = 500;
        }

        private bool TryGetDirectoryInfo(ILibrary library, PathString subpath, out IDirectoryContents contents)
        {
            var fileProvider = new PhysicalFileProvider(library.Path);
            contents = fileProvider.GetDirectoryContents(subpath.Value);
            return contents.Exists;
        }
    }
}