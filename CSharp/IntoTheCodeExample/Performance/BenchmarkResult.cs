using IntoTheCode;
using IntoTheCode.Basic.Layer;
using IntoTheCode.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntoTheCodeExample.Performance
{
    public class BenchmarkResult : NotifyChanges
    {
        public static int Version = 1;

        public static string BenchmarkFileGrammar = @"results  = {result1};
result1  = TestCode tag date size combi runs version avg max min;
TestCode = identifier;
tag      = identifier;
date     = string;
size     = int;
combi    = int;
runs     = int;
avg      = int;
max      = int;
min      = int;
version  = int;";

        public BenchmarkResult(string code, string tag, long combi)
        {
            TestCode = code;
            Tag = tag;
            Combi = combi;
            TestDate = DateTime.Now.ToShortDateString();
        }

        public BenchmarkResult(CodeElement result)
        {
            TestCode = result.Codes().First(c => c.Name == "TestCode").Value;
            Tag = result.Codes().First(c => c.Name == "tag").Value;

            TestDate = result.Codes().First(c => c.Name == "date").Value;
            InputSize = int.Parse(result.Codes().First(c => c.Name == "size").Value);
            Combi = int.Parse(result.Codes().First(c => c.Name == "combi").Value);
            TestRuns = int.Parse(result.Codes().First(c => c.Name == "runs").Value);

            TimeParse = int.Parse(result.Codes().First(c => c.Name == "avg").Value);
            TimeQuery = int.Parse(result.Codes().First(c => c.Name == "max").Value);
            MemParse = int.Parse(result.Codes().First(c => c.Name == "min").Value);
        }

        public override string ToString()
        {
            // Must fit the grammar: result1  = TestCode tag date size combi runs avg max min version;
            return string.Format("{0} {1} '{2}' {3} {4} {5} {6} {7} {8} {9}", TestCode, Tag, TestDate, InputSize,
                Combi, TestRuns, TestVersion, TimeParse, TimeQuery, MemParse);
        }

        public string GetLevelStringFromCode()
        {
            return string.Join(" ", GetLevelListFromCode());
        }

        public List<int> GetLevelListFromCode()
        {
            string letters = "abcdefghijklmn";
            int pos1 = 0;
            var levels = new List<int>();
            foreach (char ch in letters)
            {
                if (ch == 'a') continue;
                int pos2 = TestCode.IndexOf(ch);
                if (pos2 > 0)
                {
                    levels.Add(int.Parse(TestCode.Substring(pos1 + 1, pos2 - pos1 - 1)));
                    pos1 = pos2;
                }
                else
                {
                    levels.Add(int.Parse(TestCode.Substring(pos1 + 1)));
                    break;
                }
            }
        
            return levels;
        }

        public string Compare(BenchmarkResult other)
        {
            if (other == null) return string.Empty;
            string compare = string.Format("Compare to {2}-test from {0} with {1} runs\r\n", other.TestDate, other.TestRuns, other.Tag);
            if (TestCode == other.TestCode)
                compare += string.Format("Same syntax tree. Number of combinations: {0}\r\n", Combi);
            else if (Combi == other.Combi)
                compare += string.Format("Different tree! same number of combinations: {0}\r\n", Combi);
            else
                compare += string.Format("Different tree! Combinations: {0} vs {1}. Scale: {2}\r\n", Combi, other.Combi, other.Combi / Combi);

            compare += "\r\n";
            int pad = 10;
            compare += string.Format("---Time-ms---|-{0}-|-{1}-|\r\n", Tag.PadLeft(pad, '-'), other.Tag.PadLeft(pad, '-'));
            compare += string.Format("---Parse ----|-{0}-|-{1}-|\r\n", TimeParse.ToString().PadLeft(pad), other.TimeParse.ToString().PadLeft(pad));
            compare += string.Format("---Query ----|-{0}-|-{1}-|\r\n", TimeQuery.ToString().PadLeft(pad), other.TimeQuery.ToString().PadLeft(pad));
            compare += "\r\n";
            compare += string.Format("---Memory-kb-|-{0}-|-{1}-|\r\n", Tag.PadLeft(pad, '-'), other.Tag.PadLeft(pad, '-'));
            compare += string.Format("---Parse ----|-{0}-|-{1}-|\r\n", MemParse.ToString().PadLeft(pad), other.MemParse.ToString().PadLeft(pad));

            return compare;
        }

        #region properties

        public string TestCode
        {
            get { return _testCode; }
            set
            {
                if (_testCode == value) return;
                _testCode = value;
                RaisePropertyChanged(nameof(TestCode));
            }
        }
        private string _testCode;

        public string Tag
        {
            get { return _tag; }
            set
            {
                string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

                string val = string.Empty;
                if (!string.IsNullOrEmpty(value))
                    val = value.Aggregate(string.Empty, (a, s) => a += letters.Contains(s) ? s.ToString() : string.Empty);

                if (string.IsNullOrEmpty(val)) val = "master";
                if (_tag == val) return;
                _tag = val;
                RaisePropertyChanged(nameof(Tag));
            }
        }
        private string _tag;

        public int TestRuns
        {
            get { return _testRuns; }
            set
            {
                if (_testRuns == value) return;
                _testRuns = value;
                RaisePropertyChanged(nameof(TestRuns));
            }
        }
        private int _testRuns;

        public int InputSize
        {
            get { return _inputSize; }
            set
            {
                if (_inputSize == value) return;
                _inputSize = value;
                RaisePropertyChanged(nameof(InputSize));
            }
        }
        private int _inputSize;

        public long Combi
        {
            get { return _combi; }
            set
            {
                if (_combi == value) return;
                _combi = value;
                RaisePropertyChanged(nameof(Combi));
            }
        }
        private long _combi;

        public int MemorySize
        {
            get { return _memorySize; }
            set
            {
                if (_memorySize == value) return;
                _memorySize = value;
                RaisePropertyChanged(nameof(MemorySize));
            }
        }
        private int _memorySize;

        public int TimeQuery
        {
            get { return _timeQuery; }
            set
            {
                if (_timeQuery == value) return;
                _timeQuery = value;
                RaisePropertyChanged(nameof(TimeQuery));
            }
        }
        private int _timeQuery;

        public int MemParse
        {
            get { return _memParse; }
            set
            {
                if (_memParse == value) return;
                _memParse = value;
                RaisePropertyChanged(nameof(MemParse));
            }
        }
        private int _memParse;

        public int TimeParse
        {
            get { return _timeParse; }
            set
            {
                if (_timeParse == value) return;
                _timeParse = value;
                RaisePropertyChanged(nameof(TimeParse));
            }
        }
        private int _timeParse;

        public string TestDate
        {
            get { return _testDate; }
            set
            {
                if (_testDate == value) return;
                _testDate = value;
                RaisePropertyChanged(nameof(TestDate));
            }
        }
        private string _testDate;

        public int TestVersion { get { return Version; } }

        #endregion properties

    }
}
