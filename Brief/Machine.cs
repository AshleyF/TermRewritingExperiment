using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brief
{
    public class Machine
    {
        public Machine()
        {
            Stack = new Stack<dynamic>();
            Dictionary = Primitive.Dictionary();
        }

        public Stack<dynamic> Stack { get; private set; }

        public readonly Dictionary<string, IWord> Dictionary;

        public Tuple<Stack<dynamic>, int> Exec(Code code)
        {
            Stack = new Stack<dynamic>();
            Primitive.Params = 0;
            foreach (var w in code.Words.Reverse())
            {
                Console.WriteLine($"EXEC: {w}");
                Stack = w.Function(Stack);
            }
            return new Tuple<Stack<dynamic>, int>(Stack, Primitive.Params);
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
