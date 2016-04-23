using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brief
{
    public class Machine
    {
        public Machine()
        {
            Stack = new Stack<dynamic>();
            Dictionary.Add("+", new Word("+", s => { var a = s.Pop(); var b = s.Pop(); s.Push(a + b); return s; }, WordKind.Primitive, 2, 1));
            Dictionary.Add("-", new Word("-", s => { var a = s.Pop(); var b = s.Pop(); s.Push(a - b); return s; }, WordKind.Primitive, 2, 1));
            Dictionary.Add("*", new Word("*", s => { var a = s.Pop(); var b = s.Pop(); s.Push(a * b); return s; }, WordKind.Primitive, 2, 1));
            Dictionary.Add("/", new Word("/", s => { var a = s.Pop(); var b = s.Pop(); s.Push(a / b); return s; }, WordKind.Primitive, 2, 1));
            Dictionary.Add("swap", new Word("swap", s => { var a = s.Pop(); var b = s.Pop(); s.Push(a); s.Push(b); return s; }, WordKind.Primitive, 2, 2));
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