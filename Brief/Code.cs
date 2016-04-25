using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brief
{
    public class Code
    {
        private IEnumerable<IWord> ParseInternal(Machine machine, string line)
        {
            var words = line.Split(' ');
            foreach (var w in words)
            {
                Console.WriteLine("WORD: " + w);
                IWord def;
                if (machine.Dictionary.TryGetValue(w, out def))
                {
                    yield return def;
                }
                else
                {
                    double d;
                    int i;
                    if (double.TryParse(w, out d)) yield return new Word(w, s => { s.Push(d); return s; }, WordKind.Literal, 0, 1);
                    else if (int.TryParse(w, out i)) yield return new Word(w, s => { s.Push(i); return s; }, WordKind.Literal, 0, 1);
                    else yield return new Word(w, s => { s.Push(w); return s; }, WordKind.Literal, 0, 1);
                }
            }

            yield break;
        }

        public readonly Machine Machine;

        public readonly string Name;

        public readonly IList<IWord> Words;

        public Code(Machine machine, string line)
        {
            Machine = machine;
            var tokens = ParseInternal(machine, line).ToList();
            Name = tokens.First().Name; // assumes WordKind.Literal
            Words = tokens.Skip(1).ToList();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{Name} -> ");
            foreach (var w in Words)
            {
                sb.Append($"{w.Name} ");
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
    }
}
