# Kyse.AspNetCore.StaticLibrary

This repo was originally created based on the [AspNetCore.SaticFiles](https://github.com/aspnet/StaticFiles) which contains middleware for handling requests
for file system resources including files and directories.  It has been modified and expanded to support authorization
and dynamic endpoint to filesystem path mappings.

## Usage
1. Import the nuget package into your project.

2. Implement the `ILibrary` interface for your library paths.
   > You can either add this class to your DbContext and access your library paths dynamically from your database, or wrap a `Libraries` class in an `IOptions` framework to supply the data from an optional json config file.

    Library.cs:
    ```C#
    public class Library : ILibrary
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
    ```

3. Implement the `GetLibraries()` method from the `ILibraryPathProvider` interface to provide your library paths as a service to the middleware.
    > You can alternatively usee DI in the constructor to grab `IOptions<Libraries>` which should return your enumerable `Library` classes from your json config file.

    LibraryPathProvider.cs:
    ```C#
    public class LibraryPathProvider : ILibraryPathProvider
    {
        private readonly ApplicationDbContext _context;

        public LibraryPathProvider(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<ILibrary> GetLibraries() => _context.Libraries;
    }
    ```

4. Finally in `Startup.cs` seteup the middleware with a call to the extension methods.  Add `AddLibraryServer<ILibraryPathProvider>()` for the services and `UseLibraryServer()` with any `LibraryServerOptions`.

    Startup.cs:
    ```C#
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddLibraryServer<LibraryPathProvider>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseLibraryServer(new LibraryServerOptions
        {
            RequestPath = new PathString("/Library"),
            AuthorizationRequirements = new List<IAuthorizationRequirement> { new LibraryAuthorizationRequirement() },
        })
    }
    ```

## Authorization
To support abstraction from authorization, I've simplifie the check to simply call against two policies.  Use the two properties from the `LibraryServerAuthorizationPolilcy` class when registering your policies in your authorization config.
> You can make your policies as simple (via the policy builder config) or as complex as you like.  Provided below is an example of a more complex setup to narrow down authorization against the resource (library) being requested.

1. Create the authorization requirements.
    ```C#
    public class LibraryFileAuthorizationRequirement : IAuthorizationRequirement { }
    ```
2. Create your authorization handlers.
    ```C#
    public class LibraryFileAuthorizationHandler<TDbContext> : AuthorizationHandler<LibraryFileAuthorizationRequirement, LibraryServerAuthorizationResource>
        where TDbContext : DbContext
    {
        private readonly ILogger _logger;
        private readonly DbContext _dbContxt;

        public LibraryFileAuthorizationHandler(TDbContext dbContext, ILogger<LibraryFileAuthorizationHandler<TDbContext>> logger)
        {
            _logger = logger;
            _dbContxt = dbContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LibraryFileAuthorizationRequirement requirement, LibraryServerAuthorizationResource resource)
        {
            if (context.HasFailed)
                return Task.CompletedTask;

            var libraryName = resource.Library.Name;
            var hostPath = resource.Library.Path;
            var fileName = resource.HttpContext.Request.Path.Value.Split('/').Last();
            var reqPathArr = resource.HttpContext.Request.Path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var subPathArr = reqPathArr.Where((s, i) => i > 1 && i < reqPathArr.Length - 1);
            var subPath = string.Join(Path.DirectorySeparatorChar, subPathArr);
            var hostFilePath = Path.Combine(hostPath, subPath, fileName);

            // Do logic with something from above to check if they have roles/permissions/claims/etc to access the file.
            // Or check against DB using DI dbContext if you're using custom resource table, etc.

            if (context.User.HasClaim("StaticLibraryFileAccess", hostFilePath))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
    ```
3. And register your handler and requirement against the policy.
    ```C#
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, LibraryFileAuthorizationHandler<ApplicationDbContext>>();
        services.AddAuthorization(opt =>
        {
            opt.AddPolicy(LibraryServerAuthorizationPolicy.File, builder => builder.AddRequirements(new LibraryFileAuthorizationRequirement()));
        });
    }
    ```

## Options
Most of the options are inherited from `Microsoft.AspNetCore.StaticFiles` so refer to their documentation ([Working with static files in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files)) for further details.

However, there are a few more added options avaialable to control authentication and authorization access.

- AllowAnonymous - Set to true to allow anonymous access to the libraries expoesd by this middleware.
- AuthenticationSchemes - Specify an Authrneication Scheme to use for checking authentication of the request
  > If not specified, all defined authentication schemes will be checked until one authenticates with a ClaimsPrincipal.
- AuthorizationRequirements - Pass through any classes implementing IAuthorizationRequirement you've defined for protecting the libraries.

## Todo & Thoughts
- Switch Invoke to async to allow authorizeasync calls since authorize requirements could be hitting a database backend.
- Might be worth adding MapWhen to UseStaticLibrary extension against the root library path to prevent unneeded calls to our middleware code.
- Expand support to provide a service for exposing files via a temp path or via a temp auth token (media streaming?).  Might be too in depth for the purposee of this library though, as such a concept could just wrap and interface with this library via the path provider since each request will check with the path provider if a library path is valid.  However, in the current state, temp paths are limited to exposing all files in the directory, thus you'd hav to combine authorization for now to limit file access if so desired.