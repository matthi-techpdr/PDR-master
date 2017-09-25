namespace PDR.Domain.Services.VersionStorage
{
    public interface IVersionStorage<T>
    {
        bool IsWorking();
    }
}