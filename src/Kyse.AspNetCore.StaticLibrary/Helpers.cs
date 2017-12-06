// Modified by: Jared Fisher (kyse@kyse.us)
// Changes:
// - Including internal classes required for modified files.
// - Added extension methods for authentication and authorization.
// - Added extension methods for parsing library path.
//
// Original Copywrite/License Notice:
//
// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Kyse.AspNetCore.StaticLibrary
{
    internal static class Helpers
    {
        internal static IFileProvider ResolveFileProvider(IHostingEnvironment hostingEnv)
        {
            if (hostingEnv.WebRootFileProvider == null)
            {
                throw new InvalidOperationException("Missing FileProvider.");
            }
            return hostingEnv.WebRootFileProvider;
        }


        internal static bool IsGetOrHeadMethod(string method)
        {
            return HttpMethods.IsGet(method) || HttpMethods.IsHead(method);
        }

        internal static bool PathEndsInSlash(PathString path)
        {
            return path.Value.EndsWith("/", StringComparison.Ordinal);
        }

        internal static bool TryMatchLibrary(PathString path, IEnumerable<ILibrary> libraries, bool forDirectory, out PathString subpath, out ILibrary library)
        {
            if (forDirectory && !PathEndsInSlash(path))
            {
                path += new PathString("/");
            }

            foreach (var lib in libraries)
            {
                if (!path.StartsWithSegments(new PathString("/" + lib.Name), out subpath)) continue;
                library = lib;
                return true;
            }

            library = null;
            return false;
        }

        internal static bool TryMatchPath(HttpContext context, PathString matchUrl, bool forDirectory, out PathString subpath)
        {
            var path = context.Request.Path;

            if (forDirectory && !PathEndsInSlash(path))
            {
                path += new PathString("/");
            }

            if (path.StartsWithSegments(matchUrl, out subpath))
            {
                return true;
            }
            return false;
        }

        internal static bool IsAuthenticated(this HttpContext context, SharedOptionsBase options) =>
            IsAuthenticatedAsync(context, options).Result;

        internal static async Task<bool> IsAuthenticatedAsync(this HttpContext context, SharedOptionsBase options)
        {
            if (options.AllowAnonymous)
                return true;

            var authSchemes = options.AuthenticationSchemes.Any()
                                  ? options.AuthenticationSchemes
                                  : context.RequestServices.GetService<IAuthenticationSchemeProvider>()
                                           .GetAllSchemesAsync().Result.Select(scheme => scheme.Name).ToArray();
            foreach (var authScheme in authSchemes)
            {
                var cp = await context.AuthenticateAsync(authScheme);
                if (cp == null || !cp.Succeeded) continue;
                context.User = cp.Principal;
                break;
            }
            return (context.User != null && context.User.Identity.IsAuthenticated);
        }

        internal static bool IsAuthorized(this HttpContext context, SharedOptionsBase options, ILibrary library, string policy) =>
            IsAuthorizedAsync(context, options, library, policy).Result;

        internal static async Task<bool> IsAuthorizedAsync(this HttpContext context, SharedOptionsBase options, ILibrary library, string policy)
        {
            var authService = context.RequestServices.GetService<IAuthorizationService>();
            var policyProvider = context.RequestServices.GetService<IAuthorizationPolicyProvider>();
            return options.AllowAnonymous ||
                   await policyProvider.GetPolicyAsync(policy) == null ||
                   (await authService.AuthorizeAsync(context.User, new LibraryServerAuthorizationResource(context, library), policy)).Succeeded;
        }
    }
}