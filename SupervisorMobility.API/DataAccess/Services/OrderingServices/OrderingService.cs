using System.Linq.Expressions;

namespace SupervisorMobility.API.DataAccess.Services.OrderingServices
{
    public class OrderingService : IOrderingService
    {
        public Expression<Func<T, object>>? BuildJOKeySelector<T>(string? label)
        {
            var paramExp = Expression.Parameter(typeof(T));

            MemberExpression? propExp;
            UnaryExpression? convertedExp;
            Expression<Func<T, object>>? keySelectorExp = null;

            switch (label)
            {
                case "id_field":
                    propExp = Expression.PropertyOrField(paramExp, "JobObservationId");
                    convertedExp = Expression.Convert(propExp, typeof(object));
                    keySelectorExp = Expression.Lambda<Func<T, object>>(convertedExp, paramExp);
                    break;
                case "dt_field":
                    propExp = Expression.PropertyOrField(paramExp, "Distribution");
                    var innerPropExpDt = Expression.PropertyOrField(propExp, "Description"); 
                    convertedExp = Expression.Convert(innerPropExpDt, typeof(object));
                    keySelectorExp = Expression.Lambda<Func<T, object>>(convertedExp, paramExp);
                    break;
                case "op_field":
                    propExp = Expression.PropertyOrField(paramExp, "Operation");
                    var innerPropExpOp = Expression.PropertyOrField(propExp, "Description");
                    convertedExp = Expression.Convert(innerPropExpOp, typeof(object));
                    keySelectorExp = Expression.Lambda<Func<T, object>>(convertedExp, paramExp);
                    break;
                case "sd_field":
                    propExp = Expression.PropertyOrField(paramExp, "StartDate");
                    convertedExp = Expression.Convert(propExp, typeof(object));
                    keySelectorExp = Expression.Lambda<Func<T, object>>(convertedExp, paramExp);
                    break;
                case "o_field":
                    propExp = Expression.PropertyOrField(paramExp, "Operator");
                    var innerPropExO = Expression.PropertyOrField(propExp, "Name");
                    convertedExp = Expression.Convert(innerPropExO, typeof(object));
                    keySelectorExp = Expression.Lambda<Func<T, object>>(convertedExp, paramExp);
                    break;
                case "st_field":
                    propExp = Expression.PropertyOrField(paramExp, "Status");
                    convertedExp = Expression.Convert(propExp, typeof(object));
                    keySelectorExp = Expression.Lambda<Func<T, object>>(convertedExp, paramExp);
                    break;
                default:
                    propExp = Expression.PropertyOrField(paramExp, "JobObservationId");
                    convertedExp = Expression.Convert(propExp, typeof(object));
                    keySelectorExp = Expression.Lambda<Func<T, object>>(convertedExp, paramExp);
                    break;
            }
            return keySelectorExp;
        }
    }
}
