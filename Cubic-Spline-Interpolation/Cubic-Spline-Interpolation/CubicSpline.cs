using System;
using System.Collections.Generic;
namespace CubicSplineInterpolation
{
    public class CubicSpline
    {
        /*
          S(x) = a + b(x - xi) + c(x - xi)^2 + d(x - xi)^3
        */ 
        private List<double> a;
        private List<double> b;
        private List<double> c;
        private List<double> d;
        private List<double> w;

        public CubicSpline(List<double> y)
        {
            InitParameter(y);
        }
    
        double Calc(double t)
        {
            int num = (int)Math.Floor(t);
            if (num < 0)
            {
                num = 0;
            }
            else if (num >= a.Count)
            {
                num = a.Count - 1;
            }
            double dt = t - num;
            double result
            = a[num] + b[num] * dt + c[num] * Math.Pow(dt, 2.0) + d[num] * Math.Pow(dt, 3.0);
            return result;
        }

        void InitParameter(List<double> y)
        {
            foreach(int i in y)
            {
                a.Add(y[i]);

                if (i == 0)
                {
                    c.Add(0.0);
                }
                else if (i == y.Count - 1)
                {
                    c.Add(0.0);
                }
                else
                {
                    c.Add(3.0 * (a[i - 1] - 2.0 * a[i] + a[i + 1]));
                }
            }

            foreach(int i in y)
            {
                if (i == 0)
                {
                    w.Add(0.0);
                }
                else
                {
                    double tmp = 4.0 - w[i - 1];
                    c[i - 1] = (c[i] - c[i - 1]) / tmp;
                    w.Add(1.0 / tmp);
                }
            }
            for (int i = y.Count - 1; i > 0;i--)
            {
                c[i] = c[i] - c[i + 1] * w[i];
            }

            foreach(int i in y)
            {
                if(i == y.Count)
                {
                    d.Add(0.0);
                    b.Add(0.0);
                }
                else
                {
                    d.Add((c[i + 1] - c[i]) / 3.0);
                    b.Add(a[i + 1] - a[i] - c[i] - d[i]);
                }
            }
        }
    
    }
}
