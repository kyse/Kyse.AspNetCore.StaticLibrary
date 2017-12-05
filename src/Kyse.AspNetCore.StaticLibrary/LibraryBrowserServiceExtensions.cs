// Modified by: Jared Fisher (kyse@kyse.us)
// Changes:
// - Updated to support class name changes.
//
// Original Copywrite/License Notice:
//
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kyse.AspNetCore.StaticLibrary
{
    /// <summary>
    /// Extension methods for adding directory browser services.
    /// </summary>
    public static class LibraryBrowserServiceExtensions
    {
        /// <summary>
        /// Adds directory browser middleware services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
        public static IServiceCollection AddLibraryBrowser(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddWebEncoders();
            
            return services;
        }
    }
}