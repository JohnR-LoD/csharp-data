using System;
using System.IO;
using System.Text.Json;

namespace labfiles.file
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

                var testFile = new TestFile();
                
                // Note: Only one RunTest at a time is ever called.
                switch (int.Parse(args[0]))
                {
                    case 1: //Filer
                        switch (int.Parse(args[1]))
                        {
                            case 1: //Customer
                                testFile.RunTest(0, showDisplay); //readCustomerFile
                                break;
                            case 2: //Products
                                testFile.RunTest(1, showDisplay); //readCustomerData
                                testFile.RunTest(2, showDisplay); //readProductCount
                                testFile.RunTest(3, showDisplay); //readProductData
                                break;
                            case 3: //Orders
                                testFile.RunTest(4, showDisplay); //monthlyOrders
                                testFile.RunTest(5, showDisplay); //customerReport
                                break;
                        }
                        break;
                    case 2: //MySQL
                        break;
                    case 3: //Mongo
                        break;
                    case 4: //Advanced
                        break;

                    
                    
                }

            }
        }
        internal static string SaveResults(string name, object data, bool prettify = true)
        {
            var fileName = $"results.{name}.json";
            var output = JsonSerializer.Serialize(data, options: new JsonSerializerOptions
            {
                WriteIndented = prettify
            });
            File.WriteAllText(fileName, output);
            return $"You can view the results in the file {fileName}.";
        }
    }

}
