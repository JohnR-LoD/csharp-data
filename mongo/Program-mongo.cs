using System;
using System.IO;
using System.Text.Json;

namespace labfiles.mongo
{
    class Program
    {
        static void Main(string[] args)
        {
            int i;
            if ((args.Length < 2) || (!int.TryParse(args[0], out i)) || (!int.TryParse(args[1], out i)) || (int.Parse(args[0]) < 1) || (int.Parse(args[0]) > 5) || (int.Parse(args[1]) < 1) || (int.Parse(args[1]) > 5))
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
                        break;
                    case 2: //MySQL
                        break;
                    case 3: //Mongo
                        switch(int.Parse(args[1])) {
                            case 1://Connection
                                TestMongo.RunTest(0,showDisplay);
                            break;
                            case 2: 
                                TestMongo.RunTest(1,showDisplay);
                            break;
                            case 3:
                                TestMongo.RunTest(2,showDisplay);
                                TestMongo.RunTest(3,showDisplay);
                            break;
                            case 4:
                                TestMongo.RunTest(4,showDisplay);
                                TestMongo.RunTest(5,showDisplay);
                                TestMongo.RunTest(6,showDisplay);
                            break;
                            case 5:
                                TestMongo.RunTest(7,showDisplay);
                                TestMongo.RunTest(8,showDisplay);
                            break;
                        }
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
