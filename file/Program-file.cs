using System;
using System.IO;
using System.Text.Json;

namespace labfiles.file
{
    class Program
    {       
        private static int Main(string[] args)
        {
            if ( args.Length < 1 || args.Length > 2 || !int.TryParse(args[0], out var i) || i < 1 || i > TestFile.NumberOfTests )
            {
                Console.WriteLine(@"To run this console application enter the following:
                \n\n dotnet run <test#>
                \n\n Where <test#> is between 1 and 5.");
                return 1;
            }

            var showDisplay = args.Length == 2 && args[1].Contains("silent", StringComparison.OrdinalIgnoreCase);
                
            var testFile = new TestFile();
            var result = testFile.RunTest(i);
                
            var fileName = SaveResults(result.title, result.data, true);

            if (showDisplay)
            {
                Console.WriteLine(result.message);

                if (fileName != null)
                {
                    Console.WriteLine($"You can view the results in the file {fileName}.");
                }
            }

            // Flip "success" to represent an exit code.
            return result.success ? 0 : 1;
        }

        private static string SaveResults(string title, object data, bool prettify)
        {
            if (data == null) return null;
            
            var fileName = $"results.{title}.json";

            var output = JsonSerializer.Serialize(data, options: new JsonSerializerOptions
            {
                WriteIndented = prettify
            });
            File.WriteAllText(fileName, output);

            return fileName;