using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using Lucene;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;

namespace apacheLuceneAssignment
{
    public abstract class AnalyzerView
    {
        public abstract string Name { get; }

        public virtual string GetView(TokenStream tokenStream, out int numberOfTokens)
        {
            StringBuilder sb = new StringBuilder();
            Token token = tokenStream.Next();

            numberOfTokens = 0;
            while (token != null)
            {
                numberOfTokens++;
                sb.Append(GetTokenView(token));
                token = tokenStream.Next();
            }
            return sb.ToString();
        }

        protected abstract string GetTokenView(Token token);
    }
    public class TermWithOffsetsView : AnalyzerView
    {
        public override string Name
        {
            get { return "Terms With Offsets"; }
        }

        protected override string GetTokenView(Token token)
        {
            return token.TermText() + "   Start: " + token.StartOffset().ToString().PadLeft(5) + "  End: " + token.EndOffset().ToString().PadLeft(5) + "\r\n";
        }
    }
    public class TermFrequencies : AnalyzerView
    {
        public override string Name
        {
            get { return "Term Frequencies"; }
        }

        Dictionary<string, int> termDictionary = new Dictionary<string, int>();

        public override string GetView(TokenStream tokenStream, out int numberOfTokens)
        {
            StringBuilder sb = new StringBuilder();
            Token token = tokenStream.Next();

            numberOfTokens = 0;
            while (token != null)
            {
                numberOfTokens++;

                if (termDictionary.Keys.Contains(token.TermText()))
                    termDictionary[token.TermText()] = termDictionary[token.TermText()] + 1;
                else
                    termDictionary.Add(token.TermText(), 1);

                token = tokenStream.Next();
            }

            foreach (var item in termDictionary.OrderBy(x => x.Key))
            {
                sb.Append(item.Key + " [" + item.Value + "]   ");
            }
            termDictionary.Clear();

            return sb.ToString();
        }

        protected override string GetTokenView(Token token)
        {
            throw new NotImplementedException();
        }
    }
    public class AnalyzerInfo
    {
        /// <summary>
        /// A private field used to hold the value for LuceneAnalyzer property.
        /// </summary>
        private Analyzer _LuceneAnalyzer;

        /// <summary>
        /// Gets or sets the Lucene.Net Analyzer object.
        /// </summary>
        public Analyzer LuceneAnalyzer
        {
            get
            {
                return _LuceneAnalyzer;
            }
            set
            {
                _LuceneAnalyzer = value;
            }
        }


        /// <summary>
        /// A private field used to hold the value for Name property.
        /// </summary>
        private string _Name;

        /// <summary>
        /// Gets or sets the name of the analyzer.
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        /// <summary>
        /// A private field used to hold the value for Description property.
        /// </summary>
        private string _Description;

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzerInfo"/> class.
        /// </summary>
        /// <param name="name">the name of the analyzer.</param>
        /// <param name="analyzer">The Lucene.Net Analyzer to use.</param>
        public AnalyzerInfo(string name, string description, Analyzer analyzer)
        {
            this.Name = name;
            this.Description = description;
            this.LuceneAnalyzer = analyzer;
        }
    }
    class AnalyzeFile
    {
        private StringBuilder sb = new StringBuilder();
        private BindingList<AnalyzerInfo> AnalyzerList = new BindingList<AnalyzerInfo>();
        private BindingList<AnalyzerView> AnalyzerViews = new BindingList<AnalyzerView>();

        private void ReadDataFromFile()
        {
            Console.WriteLine("Provide the File Path to Read: ");
            string strFileLocation = Console.ReadLine();
            StreamReader source = new StreamReader(strFileLocation);

            while (source.EndOfStream == false)
            {
                sb.AppendLine(source.ReadLine());
            }
        }

        public void BuildAnalyzer()
        {
            AnalyzerList.Add(new AnalyzerInfo("Simple Analyzer", "An Analyzer that filters LetterTokenizer with LowerCaseFilter.", new Lucene.Net.Analysis.SimpleAnalyzer()));
            AnalyzerViews.Add(new TermWithOffsetsView());
            AnalyzerViews.Add(new TermFrequencies());

            // Read the Data File
            ReadDataFromFile();
        }

        public void PositonOfWOrds()
        {
            Lucene.Net.Analysis.Analyzer analyzer = AnalyzerList[0].LuceneAnalyzer as Analyzer;

            int termCounter = 0;
            if (analyzer != null)
            {
                AnalyzerView view = AnalyzerViews[0] as AnalyzerView;
                StringReader stringReader = new StringReader(sb.ToString());

                TokenStream tokenStream = analyzer.TokenStream("defaultFieldName", stringReader);

                String strValue = view.GetView(tokenStream, out termCounter).Trim();
                Console.WriteLine("PositonOfWOrds Details : " + strValue);
            }
        }

        public void WordCountFrequency()
        {
            Lucene.Net.Analysis.Analyzer analyzer = AnalyzerList[0].LuceneAnalyzer as Analyzer;

            int termCounter = 0;
            if (analyzer != null)
            {
                AnalyzerView view = AnalyzerViews[1] as AnalyzerView;
                StringReader stringReader = new StringReader(sb.ToString());

                TokenStream tokenStream = analyzer.TokenStream("defaultFieldName", stringReader);

                String strValue = view.GetView(tokenStream, out termCounter).Trim();
                Console.WriteLine("WordCountFrequency Details : " + strValue);
            }
            Console.WriteLine(string.Format("Total of {0} Term(s) Found.", termCounter));
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            AnalyzeFile objAnalyzeFile = new AnalyzeFile();
            objAnalyzeFile.BuildAnalyzer();
            objAnalyzeFile.PositonOfWOrds();
            objAnalyzeFile.WordCountFrequency();
            Console.Read();
        }
    }
}