namespace OrderFlow.ShipmentWorker;

public record OrderApprovedMessage(Guid OrderId, Guid DealerId, decimal Total, DateTime OccurredAt);