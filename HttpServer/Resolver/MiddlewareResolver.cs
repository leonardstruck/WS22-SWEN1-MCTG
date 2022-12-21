using System.Reflection;

namespace HttpServer.Resolver;

public static class MiddlewareResolver
{
    private static Assembly[] GetSolutionAssemblies()
    {
        // Resolve all assemblies inside solution (including pruned assemblies)
        var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)));
        return assemblies.ToArray();
    }

    public static IMiddleware[] FindMiddleware()
    {
        var assemblies = GetSolutionAssemblies();
        
        // Resolve all IEndpointControllers in assemblies
        List<Type> types = assemblies.SelectMany(x => x.GetTypes()).Where(x =>
            typeof(IMiddleware).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();

        
        List<IMiddleware> middlewares = new();

        foreach (var type in types)
        {
            // Create instance of resolved IEndpointController
            IMiddleware instance = (IMiddleware)Activator.CreateInstance(type)!;
            middlewares.Add(instance);
        }
        return middlewares.ToArray();
    }
}