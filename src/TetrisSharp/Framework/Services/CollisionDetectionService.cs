using System.Linq;
using Microsoft.Xna.Framework;
using TetrisSharp.Framework.Messaging;
using TetrisSharp.Framework.Scenes;

namespace TetrisSharp.Framework.Services
{
    /// <summary>
    /// Represents the service that detects whether two objects collides.
    /// </summary>
    /// <seealso cref="TetrisSharp.Framework.Services.GameService" />
    public sealed class CollisionDetectionService : GameService
    {
        private static readonly CollisionDetector detector = new CollisionDetector();

        public CollisionDetectionService(IScene scene)
            : base(scene) { }

        public override void Update(GameTime gameTime)
        {
            var list = Scene.Where(c => c is IVisibleComponent).ToList();
            var aArray = new IVisibleComponent[list.Count];
            var bArray = new IVisibleComponent[list.Count];
            list.CopyTo(aArray);
            list.CopyTo(bArray);
            foreach (var elementA in aArray)
            {
                foreach (var elementB in bArray)
                {
                    if (elementA.Equals(elementB))
                    {
                        continue;
                    }

                    if (detector.Collides(elementA, elementB, out var infoA, out var infoB, true))
                    {
                        var message = new CollisionDetectedMessage(elementA, elementB, infoA, infoB);
                        this.Publish(message);
                    }
                }
            }
        }
    }
}
