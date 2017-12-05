// Modified by: Jared Fisher (kyse@kyse.us)
// Changes:
// - Updated to support class name changes.
//
// Original Copywrite/License Notice:
//
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.StaticFiles.Infrastructure;

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// Options for all of the static file middleware components
    /// </summary>
    public class LibraryServerOptions : SharedOptionsBase
    {
        /// <summary>
        /// Creates a combined options class for all of the static file middleware components.
        /// </summary>
        public LibraryServerOptions()
            : base(new SharedOptions())
        {
            LibraryFileOptions = new LibraryFileOptions(SharedOptions);
            LibraryBrowserOptions = new LibraryBrowserOptions(SharedOptions);
        }

        /// <summary>
        /// Options for configuring the StaticFileMiddleware.
        /// </summary>
        public LibraryFileOptions LibraryFileOptions { get; private set; }

        /// <summary>
        /// Options for configuring the DirectoryBrowserMiddleware.
        /// </summary>
        public LibraryBrowserOptions LibraryBrowserOptions { get; private set; }
    }
}