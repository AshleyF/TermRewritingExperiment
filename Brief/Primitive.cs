using System;
using System.Collections.Generic;

namespace Brief
{
    static class Primitive
    {
        private static dynamic Pop(Stack<dynamic> stack)
        {
            if (stack.Count > 0) return stack.Pop();
            return new Param("x"); // TODO
        }

        public static IWord Word21(string name, Func<dynamic, dynamic, dynamic> fn)
        {
            Word w = null;
            Func<Stack<dynamic>, Stack<dynamic>> f = s =>
            {
                var b = Pop(s);
                var a = Pop(s);
                if (a is Param || b is Param)
                {
                    // TODO: put back unevaluated as a single value (tree)?
                }
                else
                {
                    s.Push(fn(a, b));
                }
                return s;
            };
            w = new Word(name, f, WordKind.Primitive, 2, 1);
            return w;
        }

        public static IWord Word22(string name, Func<dynamic, dynamic, Tuple<dynamic, dynamic>> fn)
        {
            Func<Stack<dynamic>, Stack<dynamic>> f = s =>
            {
                var b = Pop(s);
                var a = Pop(s);
                var t = fn(a, b);
                s.Push(t.Item1);
                s.Push(t.Item2);
                return s;
            };
            return new Word(name, f, WordKind.Primitive, 2, 2);
        }

        private static void Add(IWord word, Dictionary<string, IWord> dict)
        {
            dict.Add(word.Name, word);
        }

        public static Dictionary<string, IWord> Dictionary()
        {
            var dict = new Dictionary<string, IWord>();
            Add(Word21("+", (a, b) => a + b), dict);
            Add(Word21("-", (a, b) => a - b), dict);
            Add(Word21("*", (a, b) => a * b), dict);
            Add(Word21("/", (a, b) => a / b), dict);
            Add(Word22("swap", (a, b) => new Tuple<dynamic, dynamic>(b, a)), dict);
            return dict;
        }
    }
}