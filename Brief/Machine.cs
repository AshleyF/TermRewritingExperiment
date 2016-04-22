using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brief
{
    public enum WordKind
    {
        Literal,
        Primitive,
        Secondary,
        Param
    }

    public interface IWord
    {
        string Name { get; }
        Func<Code.INode, Code.INode> Macro { get; }
        Func<Stack<dynamic>, Stack<dynamic>> Function { get; }
        WordKind Kind { get; }
        int Arity { get; }
        int Returns { get; }
    }

    public class Word : IWord
    {
        public Word(string name, Func<Code.INode, Code.INode> macro, Func<Stack<dynamic>, Stack<dynamic>> function, WordKind kind, int arity, int returns)
        {
            Name = name;
            Macro = macro;
            Function = function;
            Kind = kind;
            Arity = arity;
            Returns = returns;
        }

        public string Name { get; private set; }
        public Func<Code.INode, Code.INode> Macro { get; private set; }
        public Func<Stack<dynamic>, Stack<dynamic>> Function { get; private set; }
        public WordKind Kind { get; private set; }
        public int Arity { get; private set; }
        public int Returns { get; private set; }

        public readonly static Func<Stack<dynamic>, Stack<dynamic>> Id = _ => _;
    }

    public class Machine
    {
        private Code.INode SwapMacro(Code.INode node)
        {
            var n = node as Code.WordNode; // assume 'swap'
            var left = n.Operands[0];
            var right = n.Operands[1]; // assume two
            var nop = new Code.WordNode(new Word("nop", Code.Node.Id, Word.Id, WordKind.Primitive, 2, 2));
            nop.Operands.Add(right);
            nop.Operands.Add(left);
            return nop;
        }

        public Machine()
        {
            Stack = new Stack<dynamic>();
            Dictionary.Add("+", new Word("+", Code.Node.Id, s => { var a = s.Pop(); var b = s.Pop(); s.Push(a + b); return s; }, WordKind.Primitive, 2, 1));
            Dictionary.Add("-", new Word("-", Code.Node.Id, s => { var a = s.Pop(); var b = s.Pop(); s.Push(a - b); return s; }, WordKind.Primitive, 2, 1));
            Dictionary.Add("*", new Word("*", Code.Node.Id, s => { var a = s.Pop(); var b = s.Pop(); s.Push(a * b); return s; }, WordKind.Primitive, 2, 1));
            Dictionary.Add("/", new Word("/", Code.Node.Id, s => { var a = s.Pop(); var b = s.Pop(); s.Push(a / b); return s; }, WordKind.Primitive, 2, 1));
            Dictionary.Add("swap", new Word("swap", SwapMacro, s => { var a = s.Pop(); var b = s.Pop(); s.Push(a); s.Push(b); return s; }, WordKind.Primitive, 2, 2));
        }

        public Stack<dynamic> Stack { get; private set; }

        public readonly Dictionary<string, IWord> Dictionary = new Dictionary<string, IWord>();

        public Stack<dynamic> Exec(Code code)
        {
            foreach (var w in code.Words.Reverse())
            {
                Stack = w.Function(Stack);
            }
            return Stack;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var v in Stack)
            {
                sb.Append($"{v} ");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}