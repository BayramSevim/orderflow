using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderFlow.Application.Common
{
    public interface IOutbox
    {
        void Add<T>(T message);
    }
}