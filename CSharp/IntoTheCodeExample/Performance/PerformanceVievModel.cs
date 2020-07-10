using IntoTheCode;
using IntoTheCode.Basic.Layer;
using IntoTheCodeExample.Basic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace IntoTheCodeExample.Performance
{
    public class PerformanceVievModel : NotifyChanges
    {

        private Parser _parserTest;
        private Parser _parserLevel;
        private CodeDocument _docLevel;
        private CodeDocument _docBenchmark;
        private string _generatedInputFull;
        private string letters = "abcdefghijklmnopqrstuvwxyz";

        public PerformanceVievModel()
        {
            Levels = "20 25";
            BenchmarkOpen();
            if (BenchmarkCompare != null)
            {
                Levels = BenchmarkCompare.GetLevelStringFromCode();
                Benchmark.Tag = BenchmarkCompare.Tag;
            }

            PrepareTest = new DelegateCommand(PrepareTestFunc);
            RunTest = new DelegateCommand(RunTestFunc);
            BenchmarkInsert = new DelegateCommand(BenchmarkInsertFunc);
            BenchmarkSave = new DelegateCommand(BenchmarkSaveFunc);
        }


        #region properties

        public string GeneratedInput
        {
            get { return _generatedInput; }
            set
            {
                if (_generatedInput == value) return;
                _generatedInput = value;
                RaisePropertyChanged(nameof(GeneratedInput));
            }
        }
        private string _generatedInput = "a\r\n b1\r\n  c1\r\n  c2";

        public string GeneratedInputAsMarkup
        {
            get { return _generatedInputAsMarkup; }
            set
            {
                if (_generatedInputAsMarkup == value) return;
                _generatedInputAsMarkup = value;
                RaisePropertyChanged(nameof(GeneratedInputAsMarkup));
            }
        }
        private string _generatedInputAsMarkup;

        public string BenchmarkFilename
        {
            get { return _benchmarkFilename; }
            set
            {
                if (_benchmarkFilename == value) return;
                _benchmarkFilename = value;
                RaisePropertyChanged(nameof(BenchmarkFilename));
            }
        }
        private string _benchmarkFilename = "Benchmark";

        public string BenchmarkFileText
        {
            get { return _resultFileText; }
            set
            {
                if (_resultFileText == value) return;
                _resultFileText = value;
                RaisePropertyChanged(nameof(BenchmarkFileText));
            }
        }
        private string _resultFileText;

        public string PreparedData
        {
            get { return _preparedData; }
            set
            {
                if (_preparedData == value) return;
                _preparedData = value;
                RaisePropertyChanged(nameof(PreparedData));
            }
        }
        private string _preparedData;

        public string Result
        {
            get { return _result; }
            set
            {
                if (_result == value) return;
                _result = value;
                RaisePropertyChanged(nameof(Result));
            }
        }
        private string _result;

        public string Levels
        {
            get { return _levels; }
            set
            {
                if (_levels == value) return;
                _levels = value;
                ReadLevels();
                RaisePropertyChanged(nameof(Levels));
            }
        }
        private string _levels;

        public BenchmarkResult Benchmark
        {
            get { return _benchmark; }
            set
            {
                if (_benchmark == value) return;
                _benchmark = value;
                RaisePropertyChanged(nameof(Benchmark));
            }
        }
        private BenchmarkResult _benchmark;

        public BenchmarkResult BenchmarkCompare { get; private set; }

        #region command properties

        public DelegateCommand PrepareTest { get; private set; }

        public DelegateCommand RunTest { get; private set; }

        public DelegateCommand BenchmarkInsert { get; private set; }

        public DelegateCommand BenchmarkSave { get; private set; }

        #endregion command properties

        private string BenchmarkFilePath
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, BenchmarkFilename) + ".txt"; }
        }

        private Parser BenchmarkFileParser {
            get
            {
                if (_benchmarkFileParser == null)
                    _benchmarkFileParser = new Parser(BenchmarkResult.BenchmarkFileGrammar);
                return _benchmarkFileParser;
            }
        }
        private Parser _benchmarkFileParser;

        #endregion properties

        #region command func

        private void PrepareTestFunc(CommandInformation ci)
        {
            PrepareInput(true);
        }

        private void RunTestFunc(CommandInformation ci)
        {
            RunTestParse();
        }

        private void BenchmarkInsertFunc(CommandInformation ci)
        {
            if (Benchmark == null)
                Result = "There is no test now. Enter levels and run test";
            else if (Benchmark.TestRuns == 0)
                Result = "The test hasn't been running";
            else
                BenchmarkFileText = Benchmark.ToString() + "\r\n" + BenchmarkFileText;
        }

        private void BenchmarkSaveFunc(CommandInformation ci)
        {
            if (string.IsNullOrEmpty(BenchmarkFileText))
            {
                Result = "String is empty";
                return;
            }

            try
            {
                File.WriteAllText(BenchmarkFilePath, BenchmarkFileText);
            }
            catch (Exception e)
            {
                Result = e.Message;
            }
        }

        #endregion command func

        #region private func

        private void ReadLevels()
        {
            if (_parserLevel == null)
            {
                _parserLevel = new Parser("levels = {int};");
            }

            string tag = Benchmark?.Tag;
            Benchmark = null;
            Result = string.Empty;
            PreparedData = string.Empty;
            _generatedInputFull = string.Empty;

            try
            {
                _docLevel = CodeDocument.Load(_parserLevel, _levels);

            }
            catch (Exception e)
            {
                PreparedData = "Can't read levels: " + e.Message;
                return;
            }

            if (_docLevel.Codes().Count() == 0)
            {
                PreparedData = "No levels found";
                return;
            }

            List<int> levels = new List<int>();
            foreach (var item in _docLevel.Codes())
                levels.Add(int.Parse(item.Value));

            if (levels.Any(i => i < 1))
            {
                PreparedData = "Levels must be 1 or greater";
                return;
            }

            if (levels.Count() > 5)
            {
                PreparedData = "Max 5 levels";
                return;
            }

            string code = string.Empty;
            for (int i = 0; i < levels.Count(); i++)
                code += string.Empty + letters[i] + levels[i];

            Benchmark = new BenchmarkResult(code, tag, levels.Aggregate(1, (mul, j) => mul *= j));
            string grammar = BuildGrammar(Benchmark.GetLevelListFromCode());
            _parserTest = new Parser(grammar);

            if (Benchmark.Combi <= 500)
            {
                PrepareInput(false);
                GeneratedInput = _generatedInputFull;
                CodeDocument doc = CodeDocument.Load(_parserTest, _generatedInputFull);
                GeneratedInputAsMarkup = doc.ToMarkup();
            }
            else
            {
                GeneratedInput = "Number of combinations > 500";
                GeneratedInputAsMarkup = "Number of combinations > 500";
                PreparedData += "Input must be prepared:\r\n";
                PreparedData += "Test grammar:\r\n" + _parserTest.GetGrammar();
            }
        }

        private void PrepareInput(bool threading)
        {
            Action prepere = () =>
            {
                _generatedInputFull = MakeInput(Benchmark.GetLevelListFromCode());
                Benchmark.InputSize = _generatedInputFull.Length;
                PreparedData = "Input is ready:\r\n";
                PreparedData += "Test grammar:\r\n" + _parserTest.GetGrammar();
            };

            if (_parserTest == null || Benchmark == null)
                PreparedData = "Type levels first";
            else if (threading)
            {
                PreparedData = "Preparing...";
                Thread thread = new Thread(new ThreadStart(prepere));
                thread.Start();
            }
            else
                prepere();
        }

        private void BenchmarkOpen()
        {

            BenchmarkFileText = string.Empty;
            if (File.Exists(BenchmarkFilePath))
            {
                try
                {
                    BenchmarkFileText = File.ReadAllText(BenchmarkFilePath);
                }
                catch (Exception e)
                {
                    Result = e.Message;
                    return;
                }

                try
                {
                    _docBenchmark = CodeDocument.Load(BenchmarkFileParser, BenchmarkFileText);

                }
                catch (Exception e)
                {
                    Result = "Can't read benchmark file: " + e.Message;
                    return;
                }
            }

            if (_docBenchmark != null && _docBenchmark.AnyNested(c => c.Name == "result1"))
                BenchmarkCompare = new BenchmarkResult(_docBenchmark.Codes().First(c => c.Name == "result1"));


        }

        private string BuildGrammar(List<int> levelList)
        {
            string grammar = @"
a  = 'a' id int {b2};
b2 = bf | b;
bf = 'b' id float;
b  = 'b' id int {c2};
c2 = cf | c;
cf = 'c' id float;
c  = 'c' id int {d2};
d2 = df | d;
df = 'd' id float;
d  = 'd' id int {e2};
e2 = ef | e;
ef = 'e' id float;
e  = 'e' id int {f2};
f2 = ff | f;
ff = 'f' id float;
f  = 'f' id int;

id = identifier;
settings
b2 collapse; 
c2 collapse; 
d2 collapse; 
e2 collapse; 
f2 collapse; ";

            return grammar;
        }

        private string MakeInput(List<int> levelList)
        {
            try
            {
                List<List<string>> sets = GetIdSets(levelList);

                StringBuilder input = new StringBuilder();

                input.Append("a abc 1\r\n");
                BuildInput(0, input, levelList, sets);
                return input.ToString();
            }
            catch (Exception ex)
            {
                Result += "Grammar doesn't work.\r\n" + ex.Message + "\r\n";
            }
            return string.Empty;

        }

        private void BuildInput(int level, StringBuilder input, List<int> levelList, List<List<string>> sets)
        {
            string indent = "       ".Substring(0, level + 1);
            char ruleName = letters[level + 1];


            foreach (string id in sets[level])
            {
                input.Append(string.Format("{0}{1} {2} {3}\r\n", indent, ruleName, id, sets[level].IndexOf(id)));

                if (level + 1 == levelList.Count) continue;

                BuildInput(level + 1, input, levelList, sets);
            }
        }

        private List<List<string>> GetIdSets(List<int> levelList)
        {
            List<List<string>> sets = new List<List<string>>();
            foreach (int i in levelList)
                sets.Add(FindCombinations(i));
            return sets;
        }

        private List<string> FindCombinations(int levelCombi)
        {
            int numberOfLetters = 1;
            while (Math.Pow(letters.Count(), numberOfLetters) < levelCombi)
                numberOfLetters++;

            List<string> result = new List<string>();

            for (int i = 0; i < levelCombi; i++)
            {
                int rest = i;
                string s = string.Empty;
                for (int j = numberOfLetters; j > 0; j--)
                {
                    int pos = rest % letters.Count();
                    s = letters[pos] + s;
                    rest = (rest - pos) / letters.Count();
                }
                result.Add(s);
            }
            return result;
        }

        private void RunTestParse()
        {
            // Function for calculating new average
            Func<float, float, double, int> newAvg = (i, old, nw) => (int)(i == 0 ? nw : old * i / (i + 1) + nw / (i + 1));

            if (_parserTest == null) Result += "Parser not set. Enter levels first\r\n";
            if (string.IsNullOrEmpty(_generatedInputFull)) Result += "No input. Prepare test first\r\n";
            if (_parserTest == null || string.IsNullOrEmpty(_generatedInputFull)) return;

            Result = "running";

            // Prepare query test
            string letters = "bcdefg";
            var levels = Benchmark.GetLevelListFromCode();
            List<List<string>> sets = GetIdSets(levels);
            long sum = 0;

            Random rnd = new Random();

            // Local function to add integer to sum.
            void AddAllCombinations2(int level, int number, TextElement elem)
            {
                if (level < sets.Count() - 1)
                {
                    string find = sets[level][rnd.Next(levels[level])];
                    AddAllCombinations2(level + 1, number,
                        elem.Codes().First(e => e.Name == letters.Substring(level, 1) &&
                        e.Codes().First().Value == find));
                }
                else
                {
                    CodeElement el = elem.Codes().First(e => e.Name == letters.Substring(level, 1) &&
                    e.Codes().First().Value == sets[level][number]);
                    sum += int.Parse(el.Codes().First(e => e.Name == "int").Value);
                }
            }

            Action parse = () =>
            {
                // ---------- MEMORY ---------------
                Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                long memoryUsage1 = currentProcess.WorkingSet64;                 
                
                // ---------- PARSING ---------------
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                CodeDocument doc = CodeDocument.Load(_parserTest, _generatedInputFull);
                stopWatch.Stop();

                // ---------- MEMORY RESULT ---------------
                currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                long memoryUsage2 = (currentProcess.WorkingSet64 - memoryUsage1) / 1024;
                if (memoryUsage2 > int.MaxValue)
                    Result += "Memory use too large.\r\n";
                else
                    Benchmark.MemParse = newAvg(Benchmark.TestRuns, Benchmark.TimeParse, memoryUsage2);

                if (stopWatch.Elapsed.TotalMilliseconds > int.MaxValue)
                    Result += "Parse time too long.\r\n";
                else
                    Benchmark.TimeParse = newAvg(Benchmark.TestRuns, Benchmark.TimeParse, stopWatch.Elapsed.TotalMilliseconds); 

                // ---------- QUERYING ---------------
                int number = -1;
                int lastLevel = levels.Last();
                stopWatch = new Stopwatch();
                stopWatch.Start();
                
                for (long l = 0; l < Benchmark.Combi; l++)
                {
                    if (++number == lastLevel) number = 0; 
                    AddAllCombinations2(0, number, doc);
                }
                stopWatch.Stop();

                long expected = ((lastLevel - 1) * lastLevel ) / 2;
                for (int i = 0; i < levels.Count - 1; i++)
                    expected *= levels[i];

                if (expected != sum)
                    throw new Exception("test sum is off");

                if (stopWatch.Elapsed.TotalMilliseconds > int.MaxValue)
                    Result += "Query time too long.\r\n";
                else
                    Benchmark.TimeQuery = newAvg(Benchmark.TestRuns, Benchmark.TimeQuery, stopWatch.Elapsed.TotalMilliseconds);

                // ---------- OUTPUT ---------------
                Benchmark.TestRuns++;
                Result = "run " + Benchmark.TestRuns;
                if (BenchmarkCompare != null)
                    Result += "\r\n" + Benchmark.Compare(BenchmarkCompare);
            };

            Thread thread = new Thread(new ThreadStart(parse));
            thread.Start();
        }

        #endregion private func

    }
}
