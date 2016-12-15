using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpPaxosRuntime.Utils
{
    public interface IStrategy
    {
        void Execute();
    }

    public interface IStrategy<T>
    {
        void Execute(T obj);
    }
}