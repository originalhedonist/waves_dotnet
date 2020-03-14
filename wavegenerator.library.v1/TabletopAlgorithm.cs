using System;

namespace wavegenerator.library
{
    public static class TabletopAlgorithm
    {
        /*            TopLength    RampLength
         *          <--------------><->
         * y|       ________________
         *  |      /ymax            \   prefixLength (same both sides)
         *  |     /                  \ <-->
         *  |____/                    \____
         *  |ymin
         * ________________________________x
         *  <-----------xmax-------------->
         * the TopFrequency doesn't necessarily have to be greater than baseFrequency - but it must be >0.
         */

        public static double GetY(double x, double xmax, double ymin, double ymax, TabletopParams p)
        {
            double prefixLength =
                (xmax - p.TopLength - 2 * p.RampLength) / 2; //length of the bit at base frequency before the first ramp
            double dy = ymax - ymin;
            if (x < prefixLength)
            {
                //before the first ramp
                return ymin;
            }
            else if (x < prefixLength + p.RampLength)
            {
                // on the first ('up') ramp
                double timeAlongRamp = x - prefixLength;
                double proportionAlongRamp = timeAlongRamp / p.RampLength;
                double proportionUpRamp = p.RampsUseSin2
                    ? Math.Pow(Math.Sin(proportionAlongRamp * Math.PI / 2), 2)
                    : proportionAlongRamp;
                return ymin + proportionUpRamp * dy;
            }
            else if (x <= prefixLength + p.RampLength + p.TopLength)
            {
                // on the tabletop
                return ymax;
            }
            else if (x <= prefixLength + 2 * p.RampLength + p.TopLength)
            {
                //on the second ('down') ramp
                double timeAlongRamp = x - prefixLength - p.RampLength - p.TopLength;
                double proportionAlongRamp = timeAlongRamp / p.RampLength;
                double proportionUpRamp = p.RampsUseSin2
                    ? Math.Pow(Math.Sin(proportionAlongRamp * Math.PI / 2), 2)
                    : proportionAlongRamp;
                return ymin + (1 - proportionUpRamp) * dy;
            }
            else
            {
                //after the second ramp
                return ymin;
            }
        }
    }
}