using Khyata.Domain.Enums;

namespace Khyata.Application.Helpers
{

    /// <summary>
    /// Encapsulates all business rules about which order status transitions are legal.
    /// Keeping this in the Domain layer ensures the rules are enforced regardless of
    /// which API (main or admin) triggers the update.
    /// </summary>
    public class OrderStatusRules
    {
        private static readonly Dictionary<OrderStatus, HashSet<OrderStatus>> Allowed = new()
            {
                [OrderStatus.New] = [OrderStatus.Pending, OrderStatus.Completed, OrderStatus.Delayed, OrderStatus.Cancelled],
                [OrderStatus.Pending] = [OrderStatus.Completed, OrderStatus.Delayed, OrderStatus.Cancelled],
                [OrderStatus.Delayed] = [OrderStatus.Pending, OrderStatus.Completed, OrderStatus.Cancelled],
                [OrderStatus.Completed] = [OrderStatus.Delivered, OrderStatus.Cancelled],
                [OrderStatus.Delivered] = [],
                [OrderStatus.Cancelled] = []
            };

            public static bool CanTransition(OrderStatus from, OrderStatus to) =>
                Allowed.TryGetValue(from, out var set) && set.Contains(to);

            public static IReadOnlySet<OrderStatus> AllowedFrom(OrderStatus from) =>
                Allowed.TryGetValue(from, out var set) ? set : new HashSet<OrderStatus>();
        
    }
}
