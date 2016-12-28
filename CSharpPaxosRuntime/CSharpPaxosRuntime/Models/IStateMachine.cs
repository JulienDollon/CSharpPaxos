using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Models.Properties;

namespace CSharpPaxosRuntime.Models
{
    public interface IStateMachine
    {
        void Update(ICommand command);
    }
}