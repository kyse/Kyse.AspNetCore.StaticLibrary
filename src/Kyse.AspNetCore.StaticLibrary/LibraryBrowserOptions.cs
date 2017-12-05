// Modified by: Jared Fisher (kyse@kyse.us)
// Changes:
// - Updated to support class name changes.
//
// Original Copywrite/License Notice:
//
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.StaticFiles;

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// Directory browsing options
    /// </summary>
    public class LibraryBrowserOptions : SharedOptionsBase
    {
        /// <summary>
        /// Enabled directory browsing for all request paths
        /// </summary>
        public LibraryBrowserOptions()
            : this(new SharedOptions())
        {
        }

        /// <summary>
        /// Enabled directory browsing all request paths
        /// </summary>
        /// <param name="sharedOptions"></param>
        public LibraryBrowserOptions(SharedOptions sharedOptions)
            : base(sharedOptions)
        {
        }

        /// <summary>
        /// The component that generates the view.
        /// </summary>
        public IDirectoryFormatter Formatter { get; set; }
    }
}