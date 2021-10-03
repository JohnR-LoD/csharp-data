﻿using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;


namespace labfiles
{
    class Program
    {
        static void Main(string[] args)
        {
            int i;
            if ((args.Length < 2) || (!int.TryParse(args[0], out i)) || (!int.TryParse(args[1], out i)) || (int.Parse(args[0]) < 1) || (int.Parse(args[0]) > 5) || (int.Parse(args[1]) < 1) || (int.Parse(args[1]) > 4))
            {
                Console.WriteLine(@"To run this console application enter the following:
                dotnet run <challenge #> <Test #>
                Where <challenge #> is:
                1 = File
                2 = MySQL
                3 = MongoDB
                4 = Advanced
                5 = Expert
                and <Test #> is between 1 and 5.");

            }
            else
            {
                var showDisplay = true;
                foreach (string arg in args)
                {
                    if (arg.ToLower() == "silent") showDisplay = false;
                }
                switch (int.Parse(args[0]))
                {
                    case 1: //File
                        switch (int.Parse(args[1]))
                        {
                            case 1: //Customer
                                TestFile.RunTest(0, showDisplay);
                                TestFile.RunTest(1, showDisplay);
                                break;
                            case 2: //Products
                                TestFile.RunTest(2, showDisplay);
                                TestFile.RunTest(3, showDisplay);
                                break;
                            case 3: //Orders
                                TestFile.RunTest(4, showDisplay);
                                TestFile.RunTest(5, showDisplay);
                                break;
                        }
                        break;
                    case 2: //MySQL
                        break;
                    case 3:
                        break;
                    case 4:
                        break;

                }

            }
        }
        internal static string SaveResults(string name, object data, bool pretify = true)
        {
            var fileName = $"results.{name}.json";
            var output = JsonSerializer.Serialize(data, options: new JsonSerializerOptions
            {
                WriteIndented = pretify
            });
            File.WriteAllText(fileName, output);
            return $"You can view the results in the file {fileName}.";
        }
    }

}
