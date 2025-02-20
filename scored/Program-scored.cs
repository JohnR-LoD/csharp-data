﻿using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using labfiles.file;
using labfiles.mysql;
using labfiles.mongo;
using labfiles.Shared;

namespace labfiles.advanced
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int i;
            if ((args.Length < 2) || (!int.TryParse(args[0], out i)) || (!int.TryParse(args[1], out i)) || (int.Parse(args[0]) < 1) || (int.Parse(args[0]) > 5) || (int.Parse(args[1]) < 1) || (int.Parse(args[1]) > 10))
            {
                Console.WriteLine(@"To run this console application enter the following:
                dotnet run <challenge #> <Test #>
                Where <challenge #> is:
                1 = File
                2 = MySQL
                3 = MongoDB
                4 = Advanced
                5 = Expert
                and <Test #> is between 1 and 10.");

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
                        break;
                    case 2: //MySQL
                        break;
                    case 3:
                        break;
                    case 4: //advanced
                        if (showDisplay)
                        {
                            switch (int.Parse(args[1]))
                            {
                                case 1: //Connect and modify relational data
                                    await TestRelational.RunTest(0, showDisplay);
                                    await TestRelational.RunTest(1, showDisplay);
                                    await TestRelational.RunTest(2, showDisplay);
                                    await TestRelational.RunTest(3, showDisplay);
                                    break;
                                case 2: //Export relational data
                                    await TestRelational.RunTest(4, showDisplay);
                                    await TestRelational.RunTest(5, showDisplay);
                                    break;
                                case 3: //Exception handling
                                    TestMongo.RunTest(0, showDisplay);
                                    TestMongo.RunTest(1, showDisplay);
                                    break;
                                case 4: //Exception handling
                                    TestReport.RunTest(0, showDisplay);
                                    TestReport.RunTest(1, showDisplay);
                                    break;
                            }
                        }
                        else
                        {
                            var resultCode = -1;
                            int test = int.Parse(args[1]);
                            var console = Console.Out;
                            Console.SetOut(new StreamWriter(File.OpenWrite(Path.GetTempFileName())));
                            if (test <= 6)
                            {
                                resultCode = await TestRelational.RunTest(test - 1, false);
                            } else if (test <=8) {
                                resultCode = TestMongo.RunTest(test-7,false);
                            } else if (test <= 10) {
                                resultCode =  TestReport.RunTest(test-9,false);
                            }
                            Console.SetOut(console);
                            Console.WriteLine(resultCode);
                        }
                        break;

                }

            }
        }
        internal static string SaveResults(string name, object data)
        {
            var fileName = $"results.{name}.json";
            var output = JsonSerializer.Serialize(data, options: new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(fileName, output);
            return $"You can view the results in the file {fileName}.";
        }
    }

}
