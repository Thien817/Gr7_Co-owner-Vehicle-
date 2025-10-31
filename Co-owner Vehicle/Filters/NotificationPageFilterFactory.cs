using Microsoft.AspNetCore.Mvc.Filters;
using Co_owner_Vehicle.Data;

namespace Co_owner_Vehicle.Filters
{
    public class NotificationPageFilterFactory : IFilterFactory, IOrderedFilter
    {
        public bool IsReusable => true;
        
        public int Order => 0;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<CoOwnerVehicleDbContext>();
            return new NotificationPageFilter(context);
        }
    }
}

