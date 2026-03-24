namespace Rentify.API.Attributes
{

    /// <summary>
    /// Decorator attribute to make Subscription Header mandatory
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequireSubscriptionHeaderAttribute : Attribute
    {

    }
}
