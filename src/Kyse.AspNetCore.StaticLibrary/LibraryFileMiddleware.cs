// Modified by: Jared Fisher (kyse@kyse.us)
// Changes:
// - Updated to support class name changes.
// - Converted Invoke to async
//
// Original Copywrite/License Notice:
//
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// Enables serving static files for a given request path
    /// </summary>
    public class LibraryFileMiddleware
    {
        private readonly LibraryFileOptions _options;
        private readonly PathString _prefixUrl;
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IContentTypeProvider _contentTypeProvider;
        private readonly ILibraryPathProvider _pathProvider;

        /// <summary>
        /// Creates a new instance of the StaticFileMiddleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="hostingEnv">The <see cref="IHostingEnvironment"/> used by this middleware.</param>
        /// <param name="options">The configuration options.</param>
        /// <param name="loggerFactory">An <see cref="ILoggerFactory"/> instance used to create loggers.</param>
        /// <param name="pathProvider">The <see cref="ILibraryPathProvider"/> instanc used to provide libraries exposed by this server.</param>
        public LibraryFileMiddleware(RequestDelegate next, IHostingEnvironment hostingEnv, IOptions<LibraryFileOptions> options, ILoggerFactory loggerFactory, ILibraryPathProvider pathProvider)
        {
            if (hostingEnv == null)
            {
                throw new ArgumentNullException(nameof(hostingEnv));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options.Value;
            _contentTypeProvider = options.Value.ContentTypeProvider ?? new LibraryContentTypeProvider();
            _pathProvider = pathProvider;
            _prefixUrl = _options.RequestPath;
            _logger = loggerFactory?.CreateLogger<LibraryFileMiddleware>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Processes a request to determine if it matches a known file, and if so, serves it.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var fileContext = new LibraryFileContext(context, _options, _prefixUrl, _logger, _contentTypeProvider, _pathProvider);

            if (!fileContext.ValidateMethod())
            {
                _logger.LogRequestMethodNotSupported(context.Request.Method);
            }
            else if (!fileContext.ValidatePath())
            {
                _logger.LogPathMismatch(fileContext.SubPath);
            }
            else if (!fileContext.LookupContentType())
            {
                _logger.LogFileTypeNotSupported(fileContext.SubPath);
            }
            else if (!fileContext.LookupFileInfo())
            {
                _logger.LogFileNotFound(fileContext.SubPath);
            }
            else
            {
                if (!context.IsAuthenticated(_options))
                {
                    _logger.LogNotAuthenticated(fileContext.Library.Name, fileContext.SubPath, context.Connection?.RemoteIpAddress?.ToString());
                    await fileContext.SendStatusAsync(Constants.Status401NotAuthenticated);
                    return;
                }
                if (!context.IsAuthorized(_options, fileContext.Library, LibraryServerAuthorizationPolicy.File))
                {
                    _logger.LogNotAuthorized(fileContext.Library.Name, fileContext.SubPath, context.User?.Identity?.Name);
                    await fileContext.SendStatusAsync(Constants.Status403NotAuthorized);
                    return;
                }

                // If we get here, we can try to serve the file
                fileContext.ComprehendRequestHeaders();
                switch (fileContext.GetPreconditionState())
                {
                    case LibraryFileContext.PreconditionState.Unspecified:
                    case LibraryFileContext.PreconditionState.ShouldProcess:
                        if (fileContext.IsHeadMethod)
                        {
                            await fileContext.SendStatusAsync(Constants.Status200Ok);
                            return;
                        }
                        if (fileContext.IsRangeRequest)
                        {
                            await fileContext.SendRangeAsync();
                            return;
                        }
                        _logger.LogFileServed(fileContext.SubPath, fileContext.PhysicalPath, context.User?.Identity?.Name);
                        await fileContext.SendAsync();
                        return;
                    case LibraryFileContext.PreconditionState.NotModified:
                        _logger.LogPathNotModified(fileContext.SubPath);
                        await fileContext.SendStatusAsync(Constants.Status304NotModified);
                        return;
                    case LibraryFileContext.PreconditionState.PreconditionFailed:
                        _logger.LogPreconditionFailed(fileContext.SubPath);
                        await fileContext.SendStatusAsync(Constants.Status412PreconditionFailed);
                        return;
                    default:
                        var exception = new NotImplementedException(fileContext.GetPreconditionState().ToString());
                        Debug.Fail(exception.ToString());
                        throw exception;
                }
            }

            await _next(context);
        }
    }
}