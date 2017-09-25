using System;

namespace SmartArch.Data.Proxy
{
    public interface IProxyAnalyzer
    {
        object GetPersist(object proxy);
        
        Type GetPersistType(object proxy);
    }
}