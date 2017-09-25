namespace PDR.Domain.StaticCollections
{
    using System.Collections.Generic;
    using System.Reflection;

    public static class ProjectsAssemblies
    {
        public static IEnumerable<Assembly> GetAssemblies()
        {
            return new List<Assembly> { Assembly.Load("PDR.Web"), Assembly.Load("PDR.Domain") };
        }
    }
}
