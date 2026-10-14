using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderFlow.Application.Common;

namespace OrderFlow.Application.Tests.Fakes
{
    public class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveCount;
        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            SaveCount++;
            return Task.CompletedTask;
        }
    }
}