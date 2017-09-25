using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Base
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IEntityWithStatus
    {
        Statuses Status { get; set; }
    }
}
