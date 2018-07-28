using System.Linq;
using System.Collections.Generic;

namespace Gnuplot
{
    /// <summary>
    /// GNUPLOT Graph Class
    /// </summary>
    public class GnuplotGraph
    {
        #region enum
        /// <summary>
        /// Key position of graph
        /// </summary>
        public enum Position
        {
            left,
            right,
            top,
            bottom,
            outside,
            below,
            unset,
        }
        #endregion



        #region Internal Constant
        private static string gnuplotExeFilePath = "gnuplot.exe";
        private string graphOutputPath = "graph.png";
        private List<GraphData> plotData = new List<GraphData>();
        #endregion



        #region Property
        //Graph Settings
        public int Width { get; set; }
        public int Height { get; set; }
        public decimal FontScale { get; set; }
        public bool SetXaxisLogScale = false;
        public bool SetYaxisLogScale = false;

        //Graph Labels
        public string GraphTitle = "";
        public string XaxisLabel = "";
        public string YaxisLabel = "";

        //Key Setting(When KeyPosition was set to 'unset', unset key of graph.)
        public Position KeyPosition = Position.below;
        #endregion



        #region Public Method
        /// <summary>
        /// Constructor of this class
        /// </summary>
        /// <param name="width">graph width</param>
        /// <param name="height">graph height</param>
        /// <param name="fontScale">graph fontscale</param>
        public GnuplotGraph(int width = 800, int height = 600, decimal fontScale = 1.2M)
        {
            this.Width = width;
            this.Height = height;
            this.FontScale = fontScale;
        }

        /// <summary>
        /// Drawing graph. This will take some seconds, so wait some soconds to open the graph file.
        /// </summary>
        public void Draw(
            bool redirectStandardInput = true,
            bool createNoWindow = true,
            bool useShellExecute = false)
        {
            //Initialization for calling GNUPLOT execution file
            var GnuConsole = new System.Diagnostics.Process();
            GnuConsole.StartInfo.FileName = gnuplotExeFilePath;
            GnuConsole.StartInfo.CreateNoWindow = createNoWindow;
            GnuConsole.StartInfo.UseShellExecute = useShellExecute;
            GnuConsole.StartInfo.RedirectStandardInput = redirectStandardInput;
            GnuConsole.Start();

            WriteCommandsForGnuConsole(GnuConsole);
            GnuConsole.StandardInput.WriteLine("exit");
            GnuConsole.WaitForExit();
        }

        /// <summary>
        /// Add Graph Data Object to this class
        /// </summary>
        /// <param name="graphData">Graph Data</param>
        public void AddGraphData(GraphData graphData)
        {
            plotData.Add(graphData);
        }

        /// <summary>
        /// Set Gnuplot Execution File Path
        /// </summary>
        /// <param name="path">Gnuplot Execution File Path</param>
        public void SetGnuplotExeFilePath(string path)
        {
            if (System.IO.File.Exists(path))
                GnuplotGraph.gnuplotExeFilePath = path;
            else
                throw new System.IO.FileNotFoundException();
        }

        /// <summary>
        /// Set graph output file path
        /// </summary>
        /// <param name="path">Path</param>
        public void SetGraphOutputPath(string path)
        {
            this.graphOutputPath = path;
        }
        #endregion



        #region Private Method
        private void WriteCommandsForGnuConsole(System.Diagnostics.Process gnuConsole)
        {
            //Output File Settings
            gnuConsole.StandardInput.WriteLine("set terminal 'pngcairo' monochrome size {0},{1} enhanced font 'Times' fontscale {2}", Width, Height, FontScale);
            gnuConsole.StandardInput.WriteLine(@"set datafile separator ','");
            //gnuConsole.StandardInput.WriteLine(@"set output '{0}'", graphOutputPath);  

            //Axis Settings
            if (SetXaxisLogScale)
                gnuConsole.StandardInput.WriteLine("set logscale x");
            if (SetYaxisLogScale)
                gnuConsole.StandardInput.WriteLine("set logscale y");

            //Label Settings
            if (!XaxisLabel.Equals(""))
                gnuConsole.StandardInput.WriteLine(@"set xlabel '{0}'", XaxisLabel);
            if (!YaxisLabel.Equals(""))
                gnuConsole.StandardInput.WriteLine(@"set ylabel '{0}'", YaxisLabel);
            if (!GraphTitle.Equals(""))
                gnuConsole.StandardInput.WriteLine(@"set title '{0}'", GraphTitle);

            //Key Settings
            if (!KeyPosition.Equals(Position.unset))
                gnuConsole.StandardInput.WriteLine("set key {0}", KeyPosition.ToString());

            //Data Plotting
            plotData.Select<GraphData, int>(
                (data, index) =>
                {
                    string plotQuery;

                    if (index.Equals(0))
                        plotQuery = string.Format(@"plot '{0}' using {1} w p title '{2}'", data.DataPath, data.PlotUsing, data.DataLabel);
                    else if (!index.Equals(plotData.Count - 1))
                        plotQuery = string.Format(@"replot '{0}' using {1} w p title '{2}'", data.DataPath, data.PlotUsing, data.DataLabel);
                    else
                    {
                        gnuConsole.StandardInput.WriteLine(@"set output '{0}'", graphOutputPath);
                        plotQuery = string.Format(@"replot '{0}' using {1} w p title '{2}'", data.DataPath, data.PlotUsing, data.DataLabel);
                    }

                    gnuConsole.StandardInput.WriteLine(plotQuery);
                    return 0;
                }
            ).ToArray();
        }
        #endregion
    }



    /// <summary>
    /// GNUPLOT Graph Data Class(CSV)
    /// </summary>
    public class GraphData
    {
        public enum Axis
        {
            linear,
            inverse,
        }

        public int XDataColumn { get; protected set; }
        public int YDataColumn { get; protected set; }
        public Axis XAxis { get; set; }
        public Axis YAxis { get; set; }
        public string DataLabel { get; protected set; }
        public string DataPath { get; protected set; }

        public string PlotUsing
        {
            get
            {
                return
                XAxis.Equals(Axis.linear) ?
                    YAxis.Equals(Axis.linear) ? string.Format("{0}:{1}", XDataColumn, YDataColumn) : string.Format("(${0}):(1.0/${1})", XDataColumn, YDataColumn) :
                    YAxis.Equals(Axis.linear) ? string.Format("(1.0/${0}):(${1})", XDataColumn, YDataColumn) : string.Format("(1.0/${0}):(1.0/${1})", XDataColumn, YDataColumn);
            }
        }


        /// <summary>
        /// Constructor of this class.
        /// </summary>
        /// <param name="dataPath">CSV-Formatted Data File Path</param>
        /// <param name="dataLabel">Data Label</param>
        /// <param name="xAxis">X-axis Type</param>
        /// <param name="yAxis">Y-axis Type</param>
        /// <param name="xDataColumn">X-data column of data file</param>
        /// <param name="yDataColumn">Y-data column of data file</param>
        public GraphData(
            string dataPath,
            string dataLabel = "",
            Axis xAxis = Axis.linear,
            Axis yAxis = Axis.linear,
            int xDataColumn = 1,
            int yDataColumn = 2
            )
        {
            DataPath = dataPath;
            DataLabel = dataLabel;
            XAxis = xAxis;
            YAxis = yAxis;
            XDataColumn = xDataColumn;
            YDataColumn = yDataColumn;
        }
    }
}