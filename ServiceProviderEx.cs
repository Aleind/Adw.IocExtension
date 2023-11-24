using ConsoleCsv2Db;
using Microsoft.Extensions.DependencyInjection;

namespace Adw.IocExtension;

public static class ServiceProviderEx
{
    public static void AddFromConfig(this IServiceCollection services, HashSet<IocItem> configs)
    {
        if (configs == null || !configs.Any())
            return;
        var join = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x=>x.GetTypes().Where(t => !(t.IsInterface || t.IsValueType)))
            .Join(configs, x => x.FullName, x => x.ClassName, (t,c)=> new { t, c })
            .Distinct() 
            .ToList();
        
        foreach (var pair in join)
        {
            var config = pair.c;
            var type = pair.t;
            var register = GetFunc(services, config.TimeOfLive);
            register(
                !string.IsNullOrEmpty(config.Interface)
                    ? type.GetInterfaces().First(x => x.FullName == config.Interface)
                    : type, type);
        }
    }

    private static Func<Type, Type, IServiceCollection> GetFunc(IServiceCollection services, ServiceLifetime configTimeOfLive)
    {
        return configTimeOfLive switch
        {
            ServiceLifetime.Transient => services.AddTransient,
            ServiceLifetime.Scoped => services.AddScoped,
            ServiceLifetime.Singleton => services.AddSingleton,
            _ => throw new ArgumentOutOfRangeException(nameof(configTimeOfLive), configTimeOfLive, null)
        };
    }
}