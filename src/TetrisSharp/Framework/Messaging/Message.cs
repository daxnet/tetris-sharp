using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisSharp.Framework.Messaging
{
    public abstract class Message : IMessage
    {
        public Guid Id { get; } = Guid.NewGuid();

        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}
