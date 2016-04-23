using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brief
{
    class Program
    {
        static void SimpleTest(Machine machine)
        {
            Console.WriteLine("Simple Test");

            var code = new Code(machine, "+ 2 * 3 4");
            Console.WriteLine($"Program: {code}");

            var res = machine.Exec(code);
            Console.WriteLine($"Result (TOS): {res.Peek()}");

            var tree = Node.Tree(code.Words);
            Console.WriteLine($"Tree:\n{tree}");
        }

        static void PartialExpression(Machine machine)
        {
            Console.WriteLine("Partial Expression");

            var code = new Code(machine, "+ *");
            Console.WriteLine($"Program: {code}");

            var tree = Node.Tree(code.Words);
            Console.WriteLine($"Tree:\n{tree}");
        }

        static void Main(string[] args)
        {
            var machine = new Machine();

            SimpleTest(machine);
            PartialExpression(machine);

            Console.ReadLine();
        }
    }
}