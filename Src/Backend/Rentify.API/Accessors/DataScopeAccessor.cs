using Rentify.Core.Enums;
using Rentify.Core.Abstractions.Accessors;

namespace Rentify.API.Accessors
{
    public class DataScopeAccessor : IDataScopeAccessor
    {
        private static readonly AsyncLocal<DataScopeHolder> _current = new();

        public DataScope DataScope => _current.Value?.Scope ?? new(DataScopeModeEnum.SubscriberAccess);

        public void SetSubscription(int? subscriptionId)
        {
            _current.Value = new DataScopeHolder
            {
                Scope = new(DataScopeModeEnum.SubscriberAccess, subscriptionId)
            };
        } 


        private class DataScopeHolder
        {
            public DataScope Scope = new(DataScopeModeEnum.SubscriberAccess);
        }
    }
}
