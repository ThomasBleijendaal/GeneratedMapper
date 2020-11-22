using System.Collections.Generic;

namespace GeneratedMapper.Abstractions
{
    public interface IMapping
    {
        string? InitializerString(string sourceInstanceName);
        string? PreConstructionInitializations();
        IEnumerable<string> NamespacesUsed();
        IEnumerable<string> MapArgumentsRequired();
    }
}
