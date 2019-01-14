using System;
using MathNet.Numerics.Integration;
using MathNet.Numerics;

namespace FinalProject
{

    /// <summary>
    /// Class for pricing Cliquet Options using the Guillaume Model.
    /// <see href="https://hal.archives-ouvertes.fr/file/index/docid/924287/filename/A_few_insights_into_cliquet_options_TGuillaume.pdf">Reference</see>
    /// </summary>
    /// <remarks>
    /// This result is not in the Heston model framework.
    /// </remarks>
    public class CliquetOptions
    {
        private double RiskFreeRate { get; set; }
        private double[] FixingDates { get; set; }
        private int Lambda { get; set; }
        private double Sigma { get; set; }
        private double StrikePrice { get; set; }
        private double InitialStockPrice { get; set; }
        private double PayoutRate { get; set; }
        private readonly double k;


        /// <summary>
        /// Constructor for the Cliquet class. 
        /// </summary>
        /// <param name="RiskFreeRate"></param>
        /// <param name="FixingDates">The dates that are used for calculating the price.</param>
        /// <param name="Lambda"> Lambda represents whether the option is put or call.</param>
        /// <param name="Sigma">Volatility.</param>
        /// <param name="StrikePrice"></param>
        /// <param name="InitialStockPrice"></param>
        /// <param name="PayoutRate">The rate by which shareholders are paid.</param>
        /// <exception cref="ArgumentException">Thrown when one of the 
        /// arguments provided to a method is not valid.</exception>
        public CliquetOptions(
        double RiskFreeRate,
        double[] FixingDates,
        int Lambda,
        double Sigma,
        double StrikePrice,
        double InitialStockPrice,
        double PayoutRate
            )
        {
            this.RiskFreeRate = RiskFreeRate;
            this.FixingDates = FixingDates;
            this.Lambda = Lambda;
            this.Sigma = Sigma;
            this.StrikePrice = StrikePrice;
            this.InitialStockPrice = InitialStockPrice;
            this.PayoutRate = PayoutRate;

            for (int i=0; i<FixingDates.Length; i++)
            {
                if (FixingDates[i] <= 0) throw new ArgumentException("--- Error: Dates must be positive. ---");

            }

            if (InitialStockPrice == 0) throw new ArgumentException("--- Error: Initial Stock Price con not be 0. ---");

            k = Math.Log(StrikePrice / InitialStockPrice);
        }

        private double Mu(int i)
        {
            return (RiskFreeRate - Sigma * Sigma / 2) * FixingDates[i];
        }

        private double MuHat(int i)
        {
            return (RiskFreeRate + Sigma * Sigma / 2) * FixingDates[i];
        }

        private double Mu(int i,int j)
        {
            return (RiskFreeRate - Sigma * Sigma / 2) * (FixingDates[j] - FixingDates[i]);
        }

        private double MuHat(int i,int j)
        {
            return (RiskFreeRate + Sigma * Sigma / 2) * (FixingDates[j] - FixingDates[i]);
        }

        private double SigmaFunction(int i)
        {
            return Sigma * Math.Sqrt(FixingDates[i]);
        }

        private double SigmaFunction(int i, int j)
        {
            return Sigma * Math.Sqrt(FixingDates[j] - FixingDates[i]);
        }

        private double Beta(int i, int j)
        {
            return Math.Sqrt(FixingDates[i] / FixingDates[j]);
        }

        private double Rho(int i, int j)
        {
            return Math.Sqrt(1 - FixingDates[i] / FixingDates[j]);
        }

        private double Rho(int i, int j, int m, int n)
        {
            return Math.Sqrt((FixingDates[j] - FixingDates[i]) / (FixingDates[n] - FixingDates[m]));
        }

        private double N(double x)
        {
            return Math.Exp(-x * x / 2) / Math.Sqrt(2 * Math.PI);
        }

        private double Phi1(double a)
        {
            Func<double, double> TheFunction = t => Math.Exp(-t * t / 2);

            double Integral = SimpsonRule.IntegrateComposite(TheFunction, -100, a, 100);

            return Integral / Math.Sqrt(2 * Math.PI);
        }

        private double Phi2(double a1, double a2, double Theta)
        {
            Func<double, double, double> TheFunction = (x, y) =>
            Math.Exp(-(x * x + y * y - 2 * Theta * x * y) / (2 * (1 - Theta * Theta))) /
            (2 * Math.PI * Math.Sqrt(1 - Theta * Theta));

            double Integral = Integrate.OnRectangle(TheFunction, -100, a1, -100, a2);
            return Integral;
        }

        private double Phi3(double b1, double b2, double b3, double Theta1, double Theta2)
        {

            Func<double, double> TheFunction = x =>
            Math.Exp(-x * x / 2) / Math.Sqrt(2 * Math.PI) *
            N((b1 - Theta1 * x) / Math.Sqrt(1 - Theta1 * Theta1)) *
            N((b3 - Theta2 * x) / Math.Sqrt(1 - Theta2 * Theta2));

            double Integral = SimpsonRule.IntegrateComposite(TheFunction, -100, b2, 100);
            return Integral;
        }
        /// <summary>
        /// This is where all the functions come together to produce the final price of the option.
        /// </summary>
        /// <returns>
        /// The price of the option as a double precision number.
        /// </returns>
        /// <remarks>
        /// The double integrals are achieved using MathNet.Numerics.Integration.OnRectagle.
        /// </remarks>
        public double CalculatePrice()
        {
            double Aux1 = Lambda * Math.Exp(-RiskFreeRate * (FixingDates[2] - FixingDates[0]) - PayoutRate * FixingDates[0]);

            double Aux2 =  InitialStockPrice * Phi1(Lambda * (-k + MuHat(0)) / SigmaFunction(0));

            double Aux3 = Phi2(-Lambda * Mu(0, 1) / SigmaFunction(0, 1), -Lambda * Mu(0, 2) / SigmaFunction(0, 2), Rho(0, 1, 0, 2));

            double Aux4 = -Lambda * Math.Exp(-RiskFreeRate * FixingDates[2]) * StrikePrice * Phi1(Lambda * (-k + Mu(0)) / SigmaFunction(0));

            double Aux5 = Phi2(Lambda * -Mu(0, 1) / SigmaFunction(0, 1), Lambda * -Mu(0, 2) / SigmaFunction(0, 2), Rho(0, 1, 0, 2));

            double Aux6 = Lambda * Math.Exp(-RiskFreeRate * (FixingDates[2] - FixingDates[1]) - PayoutRate * FixingDates[1]);

            double Aux7 = InitialStockPrice * Phi1(Lambda * -Mu(1, 2) / SigmaFunction(1, 2));

            double Aux8 = Phi2(Lambda * (k - MuHat(0)) / SigmaFunction(0), Lambda * (-k + MuHat(1)) / SigmaFunction(1), -Beta(0, 1));

            double Aux9 = Phi1(Lambda * (-k + MuHat(0)) / SigmaFunction(0)) * Phi1(Lambda * MuHat(0, 1) / SigmaFunction(0, 1));

            double Aux10 = -Lambda * Math.Exp(-RiskFreeRate * FixingDates[2]) * StrikePrice * Phi1(Lambda * -Mu(1, 2) / SigmaFunction(1, 2));

            double Aux11 = Phi2(Lambda * (k - Mu(0)) / SigmaFunction(0), Lambda * (-k + Mu(1)) / SigmaFunction(1), -Beta(0, 1));

            double Aux12 = Phi1(Lambda * (-k + Mu(0)) / SigmaFunction(0)) * Phi1(Lambda * Mu(0, 1) / SigmaFunction(0, 1));

            double Aux13 = Lambda * Math.Exp(-PayoutRate * FixingDates[2]) * InitialStockPrice;

            double Aux14 = Phi3(Lambda * (-k + MuHat(2)) / SigmaFunction(2), Lambda * MuHat(0, 2) / SigmaFunction(0, 2), Lambda * MuHat(1, 2) / SigmaFunction(1, 2), Rho(0, 2), Rho(1, 2, 0, 2));

            double Aux15 = -Lambda * Math.Exp(-RiskFreeRate * FixingDates[2]) * StrikePrice;

            double Aux16 = Phi3(Lambda * (-k + Mu(2)) / SigmaFunction(2), Lambda * Mu(0, 2) / SigmaFunction(0, 2), Lambda * Mu(1, 2) / SigmaFunction(1, 2), Rho(0, 2), Rho(1, 2, 0, 2));

            return Aux1 * Aux2 * Aux3 + Aux4 * Aux5 + Aux6 * Aux7 * (Aux8 + Aux9) + Aux10 * (Aux11 + Aux12) + Aux13 * Aux14 + Aux15 * Aux16;
        }

    }
}
