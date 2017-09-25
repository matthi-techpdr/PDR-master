using System.Collections.Generic;

namespace PDR.TaskCheckEmployeeActivity
{
    using System.Linq;

    public interface ICurrentStorage<T>
    {
        T Get();
    }
}
