// Modified by: Jared Fisher (kyse@kyse.us)
// Changes:
// - Updated to support class name changes.
//
// Original Copywrite/License Notice:
//
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// Options common to several middleware components (LibraryBrowserMiddleware and LibraryFileMiddleware)
    /// </summary>
    public class SharedOptions
    {
        private PathString _requestPath;

        /// <summary>
        /// Defaults to all request paths.
        /// </summary>
        public SharedOptions()
        {
            RequestPath = PathString.Empty;
            //AuthorizationRequirements = new List<IAuthorizationRequirement>();
        }

        /// <summary>
        /// The request path that maps to static resource libraries.
        /// </summary>
        public PathString RequestPath
        {
            get { return _requestPath; }
            set
            {
                if (value.HasValue && value.Value.EndsWith("/", StringComparison.Ordinal))
                {
                    throw new ArgumentException("Request path must not end in a slash");
                }
                _requestPath = value;
            }
        }

        /// <summary>
        /// The path provider class providing the ILibrary list.
        /// </summary>
        public ILibraryPathProvider PathProvider { get; set; }

        /// <summary>
        /// Whether unauthenticated requests are allowed or not.
        /// </summary>
        public bool AllowAnonymous { get; set; }

        /// <summary>
        /// Limit authenticated requests to specific Authentication Schemes.
        /// </summary>
        public IEnumerable<string> AuthenticationSchemes { get; set; } = new List<string>();
    }
}