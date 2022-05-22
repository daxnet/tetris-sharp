using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TetrisSharp.Models
{
    public sealed class BlockDefinition
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("description", IsNullable = true)]
        public string Description { get; set; }

        [XmlArray("rotations")]
        [XmlArrayItem("rotation")]
        public List<BlockRotation> Rotations { get; set; }

        public override string ToString() => Name;
    }
}
