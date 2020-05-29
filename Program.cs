using System;

namespace CalculatorProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            Calculator calc = new Calculator();
            if (args.Length == 0)
            {
                Console.WriteLine("Needs file name as a parameter");
                return;
            }
            calc.CalculateDataFromFile(args[0]);
        }

        
    }
}
