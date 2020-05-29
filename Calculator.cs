using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace CalculatorProgram
{
    public class Calculator
    {
        private const string run = "run ";
        private const string printResultFromFile = "add $ 0";
        private int res = 0;
        private bool successFlag = true;
        private Dictionary<char, int> dict = new Dictionary<char, int>();

        public void CalculateDataFromFile(string fileName)
        {
            var listOfLines = TransferFileToListOfLines(fileName);
            while (listOfLines.Any(x => x.StartsWith(run)))
                listOfLines = ReplaceRunLineWithFileContent(listOfLines);
            
            List<string[]> list = new List<string[]>();
            foreach (var line in listOfLines)
               list.Add(InterpreteLine(line));

            foreach (var el in list)
            {
                switch (el[0])
                {
                    case "add":
                        res = Add(GetValue(el[1]), GetValue(el[2]));
                        break;
                    case "sub":
                        res = Sub(GetValue(el[1]), GetValue(el[2]));
                        break;
                    case "mul":
                        res = Mul(GetValue(el[1]), GetValue(el[2]));
                        break;
                    case "div":
                        res = Div(GetValue(el[1]), GetValue(el[2]));
                        break;
                    case "set":
                        res = Set(Convert.ToChar(el[1]), GetValue(el[2]));
                        break;
                    case "run":
                    default:
                        successFlag = false;
                        break;
                }
                writeResult();
            }
        }

        private List<string> TransferFileToListOfLines(string fileName)
        {
            string line;
            List<string> listOfLines = new List<string>();
            if (!File.Exists(fileName))
            {
                Console.WriteLine("error");
                return listOfLines;
            }

            StreamReader file = new StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(line))
                    listOfLines.Add(line);
            }
            file.Close();
            return listOfLines;
        }

        private List<string> ReplaceRunLineWithFileContent(List<string> listOfLines)
        {
            List<string> myTempList = new List<string>();
            foreach (var line in listOfLines)
            {
                if (line.StartsWith(run))
                {
                    Regex regex = new Regex(@"(?<=\s)(.*)");
                    Match match = regex.Match(line);
                    string nextFile = match.Value;
                    myTempList.AddRange(TransferFileToListOfLines(nextFile));
                    myTempList.Add(printResultFromFile);
                }
                else
                    myTempList.Add(line);
            }
            return myTempList;
        }


        private string[] InterpreteLine(string line)
        {
            Regex regex = new Regex(@"([^\s]+)");
            MatchCollection match = regex.Matches(line);
            string[] lineArgs = new string[3];
            if (match.Count == 3)
            {
                lineArgs[0] = match[0].Value;
                lineArgs[1] = match[1].Value;
                lineArgs[2] = match[2].Value;
            }
            else
                Console.WriteLine("error: Interprete line");
            return lineArgs;
        }

        private int GetValue(string arg)
        {
            if (arg == "$")
                return res;

            Regex regex = new Regex(@"^-?[0-9]*$");
            Match match = regex.Match(arg);
            if (match.Success)
                return Int32.Parse(arg);

            regex = new Regex(@"([^\s])");
            match = regex.Match(arg);
            if (match.Success)
                return CheckInDictionary(Convert.ToChar(match.Value));

            successFlag = false;
            return 0;
        }

        private int CheckInDictionary(char c)
        {
            return dict.ContainsKey(c) ? dict[c] : 0;
        }

        private int Set(char c, int b)
        {
            if (dict.ContainsKey(c))
                dict[c] = b;
            else
                dict.Add(c, b);
            return b;
        }

        private int Add(int a, int b) => a + b;

        private int Sub(int a, int b) => a - b;

        private int Mul(int a, int b) => a * b;

        private int Div(int a, int b)
        {
            if (b == 0)
            {
                successFlag = false;
                return res;
            }
            return a / b;
        }

        private void writeResult()
        {
            if (successFlag)
                Console.WriteLine($"res: {res}");
            else
            {
                Console.WriteLine("error");
                successFlag = true;
            }
        }
    }
}
