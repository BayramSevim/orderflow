using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderFlow.Application.Orders.Events;

public record OrderApprovedEvent(Guid OrderId, Guid DealerId, decimal Total, DateTime OccurredAt);