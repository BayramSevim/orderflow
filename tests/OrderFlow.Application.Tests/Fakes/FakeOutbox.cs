using OrderFlow.Application.Common;

namespace OrderFlow.Application.Tests.Fakes
{
    public class FakeOutbox : IOutbox
    {
        public List<object> Messages { get; } = new();

        public void Add<T>(T message) => Messages.Add(message!);
    }
}