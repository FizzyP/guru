using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guru
{
    class ObjectForest
    {
        List<ObjectForestNode> Roots = new List<ObjectForestNode>();

        public string getTreeFormattedString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var root in Roots)
                root.appendTreeFormattedString(builder, "");

            return builder.ToString();
        }

        public void addRoot(ObjectForestNode node)
        {
            Roots.Add(node);
        }
    }


    class ObjectForestNode
    {
        public Object Value;
        public List<ObjectForestNode> Children = new List<ObjectForestNode>();

        public void appendTreeFormattedString(StringBuilder builder, string prefix)
        {
            builder.AppendLine(prefix + Value.ToString());
            var indentedPrefix = prefix + "  ";
            foreach (var child in Children)
            {
                child.appendTreeFormattedString(builder, indentedPrefix);
            }
        }
    }
}
