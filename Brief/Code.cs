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
    }
}