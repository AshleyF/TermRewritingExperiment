using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brief
{
    public enum NodeKind
    {
        Root,
        Param,
        Word
    }

    public interface INode
    {
        NodeKind Kind { get; }
    }

    public abstract class Node : INode
    {
        public abstract NodeKind Kind { get; }

        private static IEnumerable<string> ParamNames()
        {
            var sb = new StringBuilder();
            var i = 0;
            while (true)
            {
                var j = i;
                sb.Clear();
                do
                {
                    sb.Append((char)('A' + j % 26));
                    j /= 26;
                } while (j > 0);
                yield return sb.ToString();
                i++;
            }
        }

        private static Func<string> NextParamNameFunc()
        {
            var names = ParamNames().GetEnumerator();
            return new Func<string>(() => { names.MoveNext(); return names.Current; });
        }

        public static INode Tree(IList<IWord> words)
        {
            var nextName = NextParamNameFunc();
            Action<INode, IList<IWord>> append = null;
            append = (node, list) =>
            {
                switch (node.Kind)
                {
                    case NodeKind.Root:
                        var r = new WordNode(list[0]);
                        append(r, list.Skip(1).ToList());
                        (node as RootNode).Node = r;
                        break;
                    case NodeKind.Param:
                        break;
                    case NodeKind.Word:
                        var n = node as WordNode;
                        var arity = n.Operation.Arity;
                        var remaining = list.Skip(arity).ToList();
                        for (var i = 0; i < arity; i++)
                        {
                            if (list.Count > i)
                            {
                                var m = new WordNode(list[i]); // TODO: could be param?
                                append(m, remaining);
                                n.Operands.Add(m);
                            }
                            else
                            {
                                n.Operands.Add(new ParamNode(nextName()));
                            }
                        }
                        break;
                }
            };
            var root = new RootNode();
            append(root, words);
            return root.Node;
        }
    }

    public class RootNode : Node
    {
        public INode Node;

        public override NodeKind Kind { get { return NodeKind.Root; } }
    }

    public class ParamNode : Node
    {
        public readonly string Name;

        public ParamNode(string name)
        {
            Name = name;
        }

        public override NodeKind Kind { get { return NodeKind.Param; } }
    }

    public class WordNode : Node
    {
        public WordNode(IWord op)
        {
            Operation = op;
        }

        public readonly IWord Operation;
        public readonly IList<INode> Operands = new List<INode>();

        public override NodeKind Kind { get { return NodeKind.Word; } }

        public override string ToString()
        {
            var sb = new StringBuilder();
            Action<INode> print = null;
            print = (node) =>
            {
                switch (node.Kind)
                {
                    case NodeKind.Root:
                        break;
                    case NodeKind.Param:
                        sb.Append($"{(node as ParamNode).Name} ");
                        break;
                    case NodeKind.Word:
                        var n = node as WordNode;
                        switch (n.Operation.Kind)
                        {
                            case WordKind.Literal:
                            case WordKind.Param:
                                sb.Append($"{n.Operation.Name} ");
                                break;
                            case WordKind.Primitive:
                            case WordKind.Secondary:
                                sb.Append($"({n.Operation.Name} ");
                                foreach (var m in n.Operands) print(m);
                                sb.Remove(sb.Length - 1, 1);
                                sb.Append(") ");
                                break;
                        }
                        break;
                }
            };
            print(this);
            return sb.ToString();
        }
    }
}
