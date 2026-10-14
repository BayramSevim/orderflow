using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using OrderFlow.Application.Common;
using OrderFlow.Infrastructure.Persistence;

namespace OrderFlow.Infrastructure.Outbox
{
    public class EfOutbox : IOutbox
    {
        private readonly OrderFlowDbContext _context;

        public EfOutbox(OrderFlowDbContext context) => _context = context;

        public void Add<T>(T message)
        {
            _context.OutboxMessages.Add(new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = typeof(T).Name,
                Payload = JsonSerializer.Serialize(message),
                OccurredAt = DateTime.UtcNow
            });
        }
    }
}