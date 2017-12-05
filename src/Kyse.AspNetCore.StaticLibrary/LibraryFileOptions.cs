// Modified by: Jared Fisher (kyse@kyse.us)
// Changes:
// - Updated to support class name changes.
//
// Original Copywrite/License Notice:
//
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.StaticFiles;

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// Options for serving static files
    /// </summary>
    public class LibraryFileOptions : SharedOptionsBase
    {
        /// <summary>
        /// Defaults to all request paths
        /// </summary>
        public LibraryFileOptions() : this(new SharedOptions())
        {
        }

        /// <summary>
        /// Defaults to all request paths
        /// </summary>
        /// <param name="sharedOptions"></param>
        public LibraryFileOptions(SharedOptions sharedOptions) : base(sharedOptions)
        {
            OnPrepareResponse = _ => { };
        }

        /// <summary>
        /// Used to map files to content-types.
        /// </summary>
        public IContentTypeProvider ContentTypeProvider { get; set; }

        /// <summary>
        /// The default content type for a request if the ContentTypeProvider cannot determine one.
        /// None is provided by default, so the client must determine the format themselves.
        /// http://www.w3.org/Protocols/rfc2616/rfc2616-sec7.html#sec7
        /// </summary>
        public string DefaultContentType { get; set; }

        /// <summary>
        /// If the file is not a recognized content-type should it be served?
        /// Default: false.
        /// </summary>
        public bool ServeUnknownFileTypes { get; set; }

        /// <summary>
        /// Called after the status code and headers have been set, but before the body has been written.
        /// This can be used to add or change the response headers.
        /// </summary>
        public Action<LibraryFileResponseContext> OnPrepareResponse { get; set; }
    }
}