using System;
using System.Collections.Generic;
using Gnuplot;

namespace CubicSplineInterpolation
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            CubicSplineInterpolationTest();
            //GnuplotGraphDrawTest();
        }

        public static void CubicSplineInterpolationTest()
        {
            Console.WriteLine("Cubic Spline sample");
            var sx = new List<double>() { 0.0, 1.0, 2.0, 3.0 };
            var sy = new List<double>() { 2.7, 6.0, 3.0, 6.5 };

            var CubicSpline = new CubicSpline(sy);
            var rx = new List<double>() { };
            var ry = new List<double>() { };

            for (double i = 0; i <= 3.2; i += 0.1)
            {
                rx.Add(i);
                ry.Add(CubicSpline.Calc(i));
            }

            string file_path = System.IO.Path.Combine(@"./../../../output/test.txt");
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file_path))
            for(int i = 0; i <= ry.Count - 1; i++)
            {

                sw.WriteLine(String.Format("{0} {1}", rx[i], ry[i]));
            }
            
        }

        public static void GnuplotGraphDrawTest()
        {
            var gnuGraph = new GnuplotGraph();
            var sampleDataInc = new GraphData(@"[datafilepath]",
                "NaCl Singlecrystal #1 increase", GraphData.Axis.inverse);
            var sampleDataDec = new GraphData(@"[datafilepath",
                "NaCl Singlecrystal #1 decrease", GraphData.Axis.inverse);

            gnuGraph.AddGraphData(sampleDataInc);
            gnuGraph.AddGraphData(sampleDataDec);
            gnuGraph.Draw();
        }
    }
}
