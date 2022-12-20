using System.Reflection;

namespace HttpServer;

public static class EndpointResolver
{
    private static Assembly[] GetSolutionAssemblies()
    {
        // Resolve all assemblies inside solution (including pruned assemblies)
        var assemblies = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x)));
        return assemblies.ToArray();
    }

    public static IEndpointController[] FindEndpointControllers()
    {
        var assemblies = GetSolutionAssemblies();
        
        // Resolve all IEndpointControllers in assemblies
        List<Type> types = assemblies.SelectMany(x => x.GetTypes()).Where(x =>
            typeof(IEndpointController).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();

        
        List<IEndpointController> endpoints = new();

        foreach (var type in types)
        {
            // Create instance of resolved IEndpointController
            IEndpointController instance = (IEndpointController)Activator.CreateInstance(type)!;
            endpoints.Add(instance);
        }
        return endpoints.ToArray();
    }
}