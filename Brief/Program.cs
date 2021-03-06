﻿using System;

namespace Brief
{
    class Program
    {
        static void Test(string name, string source)
        {
            Console.WriteLine(name);

            var machine = new Machine();

            var code = new Code(machine, source);
            Console.WriteLine($"Program: {code}");

            var res = machine.Exec(code);
            var stack = res.Item1;
            var pcount = res.Item2;
            Console.Write($"Result: ");
            foreach (var v in stack) Console.Write("{0} ", v); // ($"{v} "); // invalid IL error in Mono?!
            Console.WriteLine($" ({pcount})");

            var tree = Node.Tree(code.Words);
            Console.WriteLine($"Tree:\n{tree}");
        }

        static void Main(string[] args)
        {
            // Test("Simple Test", "+ 2 * 3 4");
            // Test("Partial Expression", "+ *");
            Test("literal", "pi 3.14159");
            Test("square", "square * dup");
            // Test("area", "area * pi square");

            Console.ReadLine();
        }
    }
}
