using System;
using System.IO;
using System.Text;

namespace RetailLockboxAssessment {
    class Program {
        private static HashSet<int> intNums = new HashSet<int>();
        private static HashSet<float> floatNums = new HashSet<float>();
        private static StreamReader FileReader = StreamReader.Null;
        private static string FileDate = "XXXXXX";
        private static string FilePath = "C:\\retaillockbox\\XXXXXXImport.txt";

        public static void Main(string[] args) {
            FileDate = ParseDate(args);
            OpenFile();
            ReadNumbers();
            WriteCSV();
        }

        private static string ParseDate(string[] args) {
            string inputDate = args[0];
            try {
                if(args.Length != 1) { throw new ArgumentException("invalid arg count"); }
                if(
                    inputDate.Length != 8 ||
                    !int.TryParse(inputDate, out _)
                ) { throw new ArgumentException("\"" + inputDate + "\" date invalid"); }
            } catch(ArgumentException ex) {
                Console.WriteLine("Exception: " + ex.Message);
                Console.WriteLine("Please provide the file date");
                Console.WriteLine("Usage: Program <YYYYMMDD>");
                Environment.Exit(1);
            }
                StringBuilder outputDateBuilder = new StringBuilder();
                outputDateBuilder.Append(inputDate[4]);
                outputDateBuilder.Append(inputDate[5]);
                outputDateBuilder.Append(inputDate[6]);
                outputDateBuilder.Append(inputDate[7]);
                outputDateBuilder.Append(inputDate[2]);
                outputDateBuilder.Append(inputDate[3]);
                string outputDate = outputDateBuilder.ToString();
                return outputDate;
        }

        private static void OpenFile() {
            FilePath = "C:\\retaillockbox\\" + FileDate + "Import.txt";
            // FilePath = "./testInput.txt";
            try {
                if(!File.Exists(FilePath)) { throw new FileNotFoundException("File not found: " + FilePath); }
            } catch(FileNotFoundException ex) {
                Console.WriteLine("Exception: " + ex.Message);
                Environment.Exit(1);
            }
            FileReader = new StreamReader(FilePath);
        }

        private static void ReadNumbers() {
            bool emptyFile = true;
            try {
                while(!FileReader.EndOfStream) {
                    string? line = FileReader.ReadLine();
                    if(line != null) {
                        emptyFile = false;
                        char[] splitChars = {' ', '\t', '%', '*', '+', ',', '-', '/', ':', '<', '=', '>'};
                        string[] words = line.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
                        foreach(string word in words) {
                            string cleanedWord = RemoveSymbols(word);
                            int intNum;
                            bool isInt = false;
                            isInt = int.TryParse(cleanedWord, out intNum);
                            if(isInt) { intNums.Add(intNum); }
                            else {
                                float floatNum;
                                bool isFloat = false;
                                isFloat = float.TryParse(cleanedWord, out floatNum);
                                if(isFloat) { floatNums.Add(floatNum); }
                            }
                        }
                    }
                }
                if(emptyFile) { throw new InvalidDataException("Empty file: " + FilePath); }
            } catch(InvalidDataException ex) {
                Console.WriteLine("Exception: " + ex.Message);
                Environment.Exit(1);
            }
        }

        private static string RemoveSymbols(string word) {
            string? newWord = null;
            int i = 0;
            foreach(char c in word) {
                if(char.IsSymbol(c)) {
                    newWord = word.Remove(i, 1);
                }
                i++;
            }
            if(newWord != null) { return newWord; }
            else { return word; }
        }

        private static void WriteCSV() {
            string csvPath = "C:\\retaillockbox\\" + FileDate + "Export.csv";
            // string csvPath = "./testOutput.csv";
            try {
                if(File.Exists(csvPath)) { throw new FileNotFoundException("File already exists: " + csvPath); }
            } catch(FileNotFoundException ex) {
                Console.WriteLine("Exception: " + ex.Message);
                Console.Write("Overwrite? (y/N): ");
                string? consoleInput = Console.ReadLine();
                if(!string.Equals(consoleInput, "Y", StringComparison.OrdinalIgnoreCase)) { Environment.Exit(0); }
            }
            string intString = string.Join(',', intNums);
            string floatString = string.Join(',', floatNums);
            string numString = intString + "," + floatString;
            StreamWriter csvWriter = new StreamWriter(csvPath);
            csvWriter.WriteLine(numString);
            csvWriter.Flush();
        }
    }
}