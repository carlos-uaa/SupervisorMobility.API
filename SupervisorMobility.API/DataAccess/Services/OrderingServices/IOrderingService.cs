using System.Linq.Expressions;

namespace SupervisorMobility.API.DataAccess.Services.OrderingServices
{
    public interface IOrderingService
    {
        Expression<Func<T, object>>? BuildJOKeySelector<T>(string? label);
    }
}
