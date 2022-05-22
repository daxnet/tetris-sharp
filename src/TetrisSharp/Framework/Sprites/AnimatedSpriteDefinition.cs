using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TetrisSharp.Framework.Sprites
{
    /// <summary>
    /// Represents the definition of an animated sprite.
    /// </summary>
    [XmlRoot]
    public sealed class AnimatedSpriteDefinition
    {
        public AnimatedSpriteDefinition()
        {
            Actions = new List<AnimatedSpriteActionDefinition>();
        }

        public List<AnimatedSpriteActionDefinition> Actions { get; set; }


        public static void Save(Stream stream, AnimatedSpriteDefinition definition)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AnimatedSpriteDefinition));
            serializer.Serialize(stream, definition);
        }

        public static AnimatedSpriteDefinition Load(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(AnimatedSpriteDefinition));
            return (AnimatedSpriteDefinition)serializer.Deserialize(stream);
        }
    }
}
