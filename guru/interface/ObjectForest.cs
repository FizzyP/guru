using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guru
{
    class ObjectForest<T>
    {
        List<ObjectForestNode<T>> Roots = new List<ObjectForestNode<T>>();

        public string getTreeFormattedString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (var root in Roots)
                root.appendTreeFormattedString(builder, "");

            return builder.ToString();
        }

        public void addRoot(ObjectForestNode<T> node)
        {
            Roots.Add(node);
        }
    }


    class ObjectForestNode<T>
    {
        public T Value;
        public List<ObjectForestNode<T>> Children = new List<ObjectForestNode<T>>();

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
