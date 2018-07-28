using System;
using System.Collections.Generic;

namespace CubicSplineInterpolation
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Cubic Spline sample");
            var sx = new List<double>() { 0.0, 1.0, 2.0, 3.0 };
            var sy = new List<double>() { 2.7, 6.0, 5.0, 6.5 };

            var CubicSpline = new CubicSpline(sy);
            var rx = new List<double>() { };
            var ry = new List<double>() { };

            for (double i = 0; i <= 3.2; i += 0.1)
            {
                rx.Add(i);
                ry.Add(CubicSpline.Calc(i));
            }
        }
    }
}
