using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCAL.Base
{
    public class GPSearch
    {
        private GPFunctor F = null;
        private double precisionSec = 1;
        private int maxIters = 50;
        private double lastC;
        private double lastValueC;
        private double errorValue = 0;
        private bool useErrorValue = false;
        private bool was_error = false;

        public GPSearch(GPFunctor functor)
        {
            F = functor;
        }

        public void setSilentErrorValue(bool se, double val)
        {
            useErrorValue = se;
            errorValue = val;
        }

        public bool wasError()
        {
            return was_error;
        }

        public double getLastValue()
        {
            return lastValueC;
        }

        public double getLastTime()
        {
            return lastC;
        }

        /// <summary>
        /// There is assumption, that there is only 1 rise in given x interval
        /// It is not defined, which rise will be resurned, if there are more than 1 rises, 
        /// </summary>
        /// <param name="from">start of time interval</param>
        /// <param name="to">end of time interval</param>
        /// <param name="limit">value for testing</param>
        /// <returns>returns time of next rise or -1 for not found</returns>
        public double findRise(double from, double to, double limit)
        {
            was_error = false;
            double a = findBellow(from, to, limit);
            if (was_error)
                return errorValue;
            double b = findAbove(a, to, limit);
            int iters = 0;
            if (was_error)
                return errorValue;

            double vala = F.getDoubleValue(a);
            double valb = F.getDoubleValue(b);

            while (Math.Abs(a - b)*86400 > precisionSec && iters < maxIters)
            {
                lastC = ( a + b ) / 2;
                lastValueC = F.getDoubleValue(lastC);

                if (lastValueC < limit)
                {
                    a = lastC;
                    vala = lastValueC;
                }
                else if (lastValueC > limit)
                {
                    b = lastC;
                    valb = lastValueC;
                }
                iters++;
            }

            return lastC;
        }

        public double findSet(double from, double to, double limit)
        {
            was_error = false;
            double a = findAbove(from, to, limit);
            if (was_error)
                return errorValue;
            double b = findBellow(a, to, limit);
            int iters = 0;
            if (was_error)
                return errorValue;

            double vala = F.getDoubleValue(a);
            double valb = F.getDoubleValue(b);

            while (Math.Abs(a - b) * 86400 > precisionSec && iters < maxIters)
            {
                lastC = (a + b) / 2;
                lastValueC = F.getDoubleValue(lastC);

                if (lastValueC > limit)
                {
                    a = lastC;
                    vala = lastValueC;
                }
                else if (lastValueC < limit)
                {
                    b = lastC;
                    valb = lastValueC;
                }
                iters++;
            }

            return lastC;
        }

        public double findMaximum(double from, double to)
        {
            was_error = false;
            double a = from;
            double b = to;
            int iterc = 0;
            int iterd = 0;
            double c, d;

            double vala = F.getDoubleValue(a);
            double valb = F.getDoubleValue(b);
            double valc, vald;

            while (Math.Abs(a - b) * 86400 > precisionSec && (iterc + iterd) < maxIters)
            {
                c = (2*a + b) / 3;
                valc = F.getDoubleValue(c);

                d = (a + 2 * b) / 3;
                vald = F.getDoubleValue(d);

                if (valc < vald)
                {
                    a = c;
                    vala = valc;
                    iterc++;
                }
                else if (valc > vald)
                {
                    b = d;
                    valb = vald;
                    iterd++;
                }
            }

            lastC = (a + b) / 2;
            lastValueC = Math.Max(vala, valb);
            if (iterd == 0 || iterc == 0)
            {
                if (useErrorValue)
                {
                    was_error = true;
                    return errorValue;
                }
                throw new Exception("Function has no maximum peak in given interval");
            }
            return lastC;
        }

        public double findMinimum(double from, double to)
        {
            was_error = false;
            double a = from;
            double b = to;
            int iterc = 0;
            int iterd = 0;
            double c, d;

            double vala = F.getDoubleValue(a);
            double valb = F.getDoubleValue(b);
            double valc, vald;

            while (Math.Abs(a - b) * 86400 > precisionSec && (iterc + iterd) < maxIters)
            {
                c = (2 * a + b) / 3;
                valc = F.getDoubleValue(c);

                d = (a + 2 * b) / 3;
                vald = F.getDoubleValue(d);

                if (valc > vald)
                {
                    a = c;
                    vala = valc;
                    iterc++;
                }
                else if (valc < vald)
                {
                    b = d;
                    valb = vald;
                    iterd++;
                }
            }
            lastC = (a + b) / 2;
            lastValueC = Math.Min(vala, valb);
            if (iterd == 0 || iterc == 0)
            {
                if (useErrorValue)
                {
                    was_error = true;
                    return errorValue;
                }
                throw new Exception("Function has no minumum peak in given interval");
            }
            return lastC;
        }


        /// <summary>
        /// This functions finds first X where value(x) is abouve given limit.
        /// Difference with rise is, that this function gives first value found
        /// above that limit, so it is not precise moment, when func(x)
        /// has rose above given limit.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public double findAbove(double from, double to, double limit)
        {
            if (F.getDoubleValue(from) > limit)
                return from;
            if (F.getDoubleValue(to) > limit)
                return to;

            double start = (from + to) / 2;
            double step = (to - from) / 2;
            int steps = 1;
            double x;

            for (int i = 0; i < 10; i++)
            {
                x = start;
                for(int a = 0; a < steps; a++)
                {
                    x += step;
                    if (F.getDoubleValue(x) > limit)
                        return x;
                }

                start = (start + from) / 2;
                step /= 2;
                steps *= 2;
            }

            throw new Exception("next value above " + limit + "not found");
        }

        /// <summary>
        /// This functions finds first X where value(x) is bellow given limit.
        /// Difference with findNextSet is, that this function gives first value found
        /// bellow that limit, so it is not precise moment, when func(x)
        /// has set bellow given limit.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public double findBellow(double from, double to, double limit)
        {
            if (F.getDoubleValue(from) < limit)
                return from;
            if (F.getDoubleValue(to) > limit)
                return to;

            double start = (from + to) / 2;
            double step = (to - from) / 2;
            int steps = 1;
            double x;

            for (int i = 0; i < 10; i++)
            {
                x = start;
                for (int a = 0; a < steps; a++)
                {
                    x += step;
                    if (F.getDoubleValue(x) < limit)
                        return x;
                }

                start = (start + from) / 2;
                step /= 2;
                steps *= 2;
            }

            throw new Exception("next value bellow " + limit + "not found");
        }

        /// <summary>
        /// Let d(min) is minimum distance of two extremes (minimums)
        /// then this should be true:
        ///     d(min) > step * 2
        /// </summary>
        /// <param name="startDay"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public double findNextMinimum(double startDay, double step)
        {
            double date = startDay;
            double next = 0;

            double[] a = new double[3];
            double[] t = new double[3];
            int i = 1;

            for (i = 0; i < 3; i++)
            {
                t[i] = date;
                a[i] = F.getDoubleValue(date);
                date += step;
            }

            if (t[1] < t[0])
                next = findMinimum(t[1], t[0]);
            else
                next = findMinimum(t[0], t[1]);
            if (next > 0)
                return next;

            while (i < maxIters)
            {
                if (a[1] < a[2] && a[1] < a[0])
                {
                    if (t[2] < t[0])
                        next = findMinimum(t[2], t[0]);
                    else
                        next = findMinimum(t[0], t[2]);
                    if (next > 0)
                        return next;
                }
                t[0] = t[1];
                t[1] = t[2];
                t[2] = date;

                a[0] = a[1];
                a[1] = a[2];
                a[2] = F.getDoubleValue(date);

                date += step;

                i++;
            }

            return -1;
        }
    }
}
