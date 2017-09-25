using System;

namespace PDR.Domain.Services.Sheduler
{
    public interface ISheduler
    {
        void RunProcessByTime(DateTime timeToRun, Action whatToDo);
    }
}
