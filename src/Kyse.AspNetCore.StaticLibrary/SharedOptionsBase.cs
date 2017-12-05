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
using Microsoft.Extensions.FileProviders;

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// Options common to several middleware components
    /// </summary>
    public abstract class SharedOptionsBase
    {
        /// <summary>
        /// Creates an new instance of the SharedOptionsBase.
        /// </summary>
        /// <param name="sharedOptions"></param>
        protected SharedOptionsBase(SharedOptions sharedOptions)
        {
            if (sharedOptions == null)
            {
                throw new ArgumentNullException(nameof(sharedOptions));
            }

            SharedOptions = sharedOptions;
        }

        /// <summary>
        /// Options common to several middleware components
        /// </summary>
        protected SharedOptions SharedOptions { get; private set; }

        /// <summary>
        /// The relative request path that maps to static resources.
        /// </summary>
        public PathString RequestPath
        {
            get { return SharedOptions.RequestPath; }
            set { SharedOptions.RequestPath = value; }
        }

        //public ILibraryPathProvider PathProvider {
        //    get { return SharedOptions.PathProvider; }
        //    set { SharedOptions.PathProvider = value; }
        //}

        public bool AllowAnonymous
        {
            get { return SharedOptions.AllowAnonymous; }
            set { SharedOptions.AllowAnonymous = value; }
        }

        public IEnumerable<string> AuthenticationSchemes
        {
            get { return SharedOptions.AuthenticationSchemes; }
            set { SharedOptions.AuthenticationSchemes = value; }
        }

        public IEnumerable<IAuthorizationRequirement> AuthorizationRequirements
        {
            get { return SharedOptions.AuthorizationRequirements; }
            set { SharedOptions.AuthorizationRequirements = value; }
        }

    }
}