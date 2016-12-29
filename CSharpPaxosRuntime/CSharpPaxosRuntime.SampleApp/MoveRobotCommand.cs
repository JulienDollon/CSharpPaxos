using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpPaxosRuntime.Models;

namespace CSharpPaxosRuntime.SampleApp
{
    class MoveRobotCommand : ICommand
    {
        public RobotMove Move { get; set; }
    }
}