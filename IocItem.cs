using Microsoft.Extensions.DependencyInjection;

namespace ConsoleCsv2Db;

public class IocItem
{
    public string ClassName { get; set; }
    public string Interface { get; set; }
    public ServiceLifetime TimeOfLive { get; set; } = ServiceLifetime.Transient;
}