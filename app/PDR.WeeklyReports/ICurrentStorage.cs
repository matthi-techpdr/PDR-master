namespace PDR.WeeklyReports
{
    public interface ICurrentStorage<T>
    {
        T Get();
    }
}
