using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisSharp.Framework.Messaging
{
    public sealed class ReachBoundaryMessage : Message
    {
        public ReachBoundaryMessage(Boundary boundary)
        {
            this.ReachedBoundary = boundary;
        }

        public Boundary ReachedBoundary { get; }
    }
}
