namespace OrderFlow.Domain.Orders
{
    public enum OrderStatus
    {
        Pending,
        Approved,
        Shipped,
        Delivered,
        Cancelled
    }
}