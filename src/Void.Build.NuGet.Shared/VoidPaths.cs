using System.IO;

namespace Void.Build.NuGet.Shared; 

public static class VoidPaths {
    public const string VOID = ".void";
    public const string CACHE = "cache";
    public const string ASSEMBLIES = "assemblies";
    
    public static readonly string ASSEMBLY_CACHE_DIR = Path.Combine(VOID, CACHE, ASSEMBLIES);
}
