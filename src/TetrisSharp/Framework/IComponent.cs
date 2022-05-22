using Microsoft.Xna.Framework;
using TetrisSharp.Framework.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TetrisSharp.Framework
{
    /// <summary>
    /// Represents that the implemented classes are game components.
    /// </summary>
    public interface IComponent : IEquatable<IComponent>
    {
        Guid Id { get; }

        bool IsActive { get; set; }

        void Update(GameTime gameTime);

    }
}
