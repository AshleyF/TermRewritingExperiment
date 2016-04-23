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
        Func<Stack<dynamic>, Stack<dynamic>> Function { get; }
        WordKind Kind { get; }
        int Arity { get; }
        int Returns { get; }
    }

    public class Word : IWord
    {
        public Word(string name, Func<Stack<dynamic>, Stack<dynamic>> function, WordKind kind, int arity, int returns)
        {
            Name = name;
            Function = function;
            Kind = kind;
            Arity = arity;
            Returns = returns;
        }

        public string Name { get; private set; }
        public Func<Stack<dynamic>, Stack<dynamic>> Function { get; private set; }
        public WordKind Kind { get; private set; }
        public int Arity { get; private set; }
        public int Returns { get; private set; }
    }
}