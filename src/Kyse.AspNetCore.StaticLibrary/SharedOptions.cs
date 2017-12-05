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
    /// Options common to several middleware components
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
            AuthorizationRequirements = new List<IAuthorizationRequirement>();
        }

        /// <summary>
        /// The request path that maps to static resources
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

        public ILibraryPathProvider PathProvider { get; set; }

        public bool AllowAnonymous { get; set; }

        public IEnumerable<string> AuthenticationSchemes { get; set; }

        public IEnumerable<IAuthorizationRequirement> AuthorizationRequirements { get; set; }

    }
}