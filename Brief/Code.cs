using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brief
{
    public class Code
    {
        private IEnumerable<IWord> ParseInternal(Machine machine, string line)
        {
            var words = line.Split(' ');
            foreach (var w in words)
            {
                IWord def;
                if (machine.Dictionary.TryGetValue(w, out def))
                {
                    yield return def;
                }
                else
                {
                    double d;
                    int i;
                    if (double.TryParse(w, out d)) yield return new Word(w, n => n, s => { s.Push(d); return s; }, WordKind.Literal, 0, 1);
                    else if (int.TryParse(w, out i)) yield return new Word(w, n => n, s => { s.Push(i); return s; }, WordKind.Literal, 0, 1);
                    else throw new Exception(String.Format("Invalid token '${w}'"));
                }
            }

            yield break;
        }

        public readonly Machine Machine;

        public readonly IList<IWord> Words;

        public Code(Machine machine, string source)
        {
            Machine = machine;
            Words = ParseInternal(machine, source).ToList();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var w in Words)
            {
                sb.Append($"{w.Name} ");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

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
            public readonly static Func<Code.INode, Code.INode> Id = _ => _;

            public abstract NodeKind Kind { get; }
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

        private IEnumerable<string> ParamNames()
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

        private Func<string> NextParamNameFunc()
        {
            var names = ParamNames().GetEnumerator();
            return new Func<string>(() => { names.MoveNext(); return names.Current; });
        }

        public INode Tree
        {
            get
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
                append(root, Words);
                return root.Node;
            }
        }

        public INode Macro(INode node)
        {
            switch (node.Kind)
            {
                case NodeKind.Root:
                case NodeKind.Param:
                    return node;
                case NodeKind.Word:
                    // TODO: recursive and interative as long as changed
                    return (node as WordNode).Operation.Macro(node);
            }
            // TODO
            return node;
        }
    }
}