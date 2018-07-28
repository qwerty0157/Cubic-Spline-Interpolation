using System;
using System.Collections.Generic;
namespace CubicSplineInterpolation
{
    public class CubicSpline
    {
        /*
          S(x) = a + b(x - xi) + c(x - xi)^2 + d(x - xi)^3 
        */
        private List<double> a = new List<double>();
        private List<double> b = new List<double>();
        private List<double> c = new List<double>();
        private List<double> d = new List<double>();
        private List<double> w = new List<double>();

        public CubicSpline(List<double> y)
        {
            InitParameter(y);
        }

        public List<double> A { get => a; set => a = value; }
        public List<double> B { get => b; set => b = value; }
        public List<double> C { get => c; set => c = value; }
        public List<double> D { get => d; set => d = value; }
        public List<double> W { get => w; set => w = value; }

        public double Calc(double t)
        {
            int num = (int)Math.Floor(t);
            if (num < 0)
            {
                num = 0;
            }
            else if (num >= A.Count)
            {
                num = A.Count - 1;
            }
            double dt = t - num;
            double result
            = A[num] + B[num] * dt + C[num] * Math.Pow(dt, 2.0) + D[num] * Math.Pow(dt, 3.0);
            return result;
        }

        void InitParameter(List<double> y)
        {
            for (int i = 0; i <= y.Count - 1; ++i)
            {
                A.Add(y[i]);
            }

            for (int i = 0; i <= y.Count - 1; ++i)
            {
                if (i == 0)
                {
                    C.Add(0.0);
                }
                else if (i == y.Count - 1)
                {
                    C.Add(0.0);
                }
                else
                {
                    C.Add(3.0 * (A[i - 1] - 2.0 * A[i] + A[i + 1]));
                }
            }

            for (int i = 0; i <= y.Count - 1; ++i)
            {
                if (i == 0)
                {
                    W.Add(0.0);
                }
                else
                {
                    double tmp = 4.0 - W[i - 1];
                    C[i - 1] = (C[i] - C[i - 1]) / tmp;
                    W.Add(1.0 / tmp);
                }
            }
            for (int i = y.Count - 1; i > 0;)
            {
                i--;
                C[i] = C[i] - C[i + 1] * W[i];
            }

            for (int i = 0; i <= y.Count - 1; ++i)
            {
                if (i == y.Count - 1)
                {
                    D.Add(0.0);
                    B.Add(0.0);
                }
                else
                {
                    D.Add((C[i + 1] - C[i]) / 3.0);
                    B.Add(A[i + 1] - A[i] - C[i] - D[i]);
                }
            }
        }

    }
}
