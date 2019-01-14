using System;
using System.Numerics;
using MathNet.Numerics.Integration;

namespace FinalProject
{
    /// <summary>
    /// Class for pricing European put and call options using the Heston formula.
    /// <see href="final-project-almedina12/HestonModel.pdf">Reference.</see>
    /// </summary>
    /// <remarks>
    /// The names of the global variables  and functions are meant to resemble the ones in
    /// the documentation for easier understanding.
    /// </remarks>
    public class HestonFormula
    {
        private double RiskFreeRate { get; }
        private double StrikePrice { get; set; }
        private double InitialStockPrice { get;}
        private double Kappa { get; } 
        private double Theta { get; } 
        private double Sigma { get; } 
        private double Rho { get; } 
        private double Nu { get; set; }
        private double[] u { get; }
        private double[] b { get; }
        private double a { get; }
        private Complex i = Complex.ImaginaryOne;
        private int NumModelParams { get; }


        /// <summary>
        /// Constructor for the class. Global variables <paramref name="u"/>, <paramref name="a"/> 
        /// and <paramref name="b"/> are calculated.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when one of the 
        /// arguments provided to a method is not valid.</exception>
        public HestonFormula(
        double RiskFreeRate,
        double InitialStockPrice,
        double Kappa,
        double Theta,
        double Sigma,
        double Rho,
        double Nu)
        {
            this.RiskFreeRate = RiskFreeRate;
            this.InitialStockPrice = InitialStockPrice;
            this.Kappa = Kappa;
            this.Theta = Theta;

            if (Sigma < 0)
                throw new ArgumentException("--- Error: Sigma must not be negative. ---");
            this.Sigma = Sigma;

            this.Rho = Rho;
            this.Nu = Nu;
            NumModelParams = 5;

            b = new double[2];
            u = new double[2];

            a = Kappa * Theta;
            b[0] = Kappa - Sigma * Rho;
            b[1] = Kappa;
            u[0] = 0.5;
            u[1] = -0.5;

        }

        /// <summary>
        /// Constructor for the class. Global variables <paramref name="u"/>, <paramref name="a"/> 
        /// and <paramref name="b"/> are calculated.
        /// </summary>
        /// <param name="Param"> Array arranged in the following order: Kappa, Theta, Sigma, Rho and Nu .</param>
        /// <remarks>
        /// No exception is thrown to prevent errors during calibration.
        /// </remarks>
        public HestonFormula(
        double RiskFreeRate,
        double InitialStockPrice,

        double[] Param)
        {
            this.RiskFreeRate = RiskFreeRate;
            this.InitialStockPrice = InitialStockPrice;

            Kappa = Param[0];
            Theta = Param[1];

            Sigma = Param[2];

            Rho = Param[3];
            Nu = Param[4];
            NumModelParams = 5;

            b = new double[2];
            u = new double[2];

            a = Kappa * Theta;

            b[0] = Kappa - Sigma * Rho;
            b[1] = Kappa;
            u[0] = 0.5;
            u[1] = -0.5;

        }


        private Complex d(int j, double Phi)
        {
            Complex Aux1 = Complex.Pow(Rho * Sigma * Phi * i - b[j], 2);
            Complex Aux2 = Sigma * Sigma * (2 * u[j] * Phi * i - Phi * Phi);
            return Complex.Sqrt(Aux1 - Aux2);
        }



        private Complex g(int j, double Phi)
        {
            Complex Aux1 = b[j] - Rho * Sigma * Phi * i - d(j, Phi);
            Complex Aux2 = b[j] - Rho * Sigma * Phi * i + d(j, Phi);

            if (Aux2 == 0) throw new DivideByZeroException();

            return Aux1 / Aux2;
        }


        private Complex C(int j, double Tau, double Phi)
        {
            Complex Aux1 = RiskFreeRate * Phi * i * Tau;
            Complex Aux2 = (b[j] - Rho * Sigma * Phi * i - d(j, Phi)) * Tau;
            Complex Aux3 = 1 - g(j, Phi) * Complex.Exp(-Tau * d(j, Phi));
            Complex Aux4 = 1 - g(j, Phi);

            if (Aux4 == 0) throw new DivideByZeroException();

            return Aux1 + a / Complex.Pow(Sigma, 2) * (Aux2 - 2 * Complex.Log(Aux3 / Aux4));
        }


        private Complex D(int j, double Tau, double Phi)
        {
            Complex Aux1 = b[j] - Rho * Sigma * Phi * i - d(j, Phi);
            Complex Aux2 = 1 - Complex.Exp(-Tau * d(j, Phi));
            Complex Aux3 = 1 - g(j, Phi) * Complex.Exp(-Tau * d(j, Phi));

            if (Aux3 == 0) throw new DivideByZeroException();

            return Aux1 / Complex.Pow(Sigma, 2) * (Aux2 / Aux3);

        }

        /// <summary>
        /// Formula for the function Phi found in the documentation.
        /// </summary>
        /// <remarks>
        /// There is a slight change with the initial formula shown in the documentation.
        /// t is takes as 0 and T is taken as a variable.
        /// </remarks>
        private Complex PhiFunction(int j, double T, double x, double Phi)
        {
            return Complex.Exp(C(j, T, Phi) + D(j, T, Phi) * Nu + i * Phi * x);
        }

        /// <summary>
        /// Formula for the P found in the documentation.
        /// </summary>
        /// <returns>
        /// A double precision number.
        /// </returns>
        /// <param name="j">Index of arrays. Either 0 or 1.</param>
        /// <param name="T">A double precision number.</param>
        /// <param name="x">A double precision number.</param>
        /// <remarks>
        /// <para>The MathNet.Numerics.Integration.SimpsonRule is used to find the integral.</para
        /// </remarks>
        private double P(int j, double T, double x)
        {
            double Integral;
            double IntervalBegin = 0.00001;
            double IntervalEnd = 100;
            int NumPartitions = 100;
            Func<double, double> AuxFunction;


                AuxFunction = Phi => (Complex.Exp(-i * Phi * Math.Log(StrikePrice)) * PhiFunction(j, T, x, Phi) / (i * Phi)).Real;
                Integral = SimpsonRule.IntegrateComposite(AuxFunction,IntervalBegin, IntervalEnd, NumPartitions);


            return 0.5 + (1 / Math.PI) * Integral;
        }


        /// <summary>
        /// Formula for the c found in the documentation.
        /// </summary>
        /// <returns>
        /// The European Call price of the option.
        /// </returns>
        /// <param name="StrikePrice">The strike price of the option.</param>
        /// <param name="OptionExcercise">The exercise date of the option.</param>
        /// <remarks>
        /// There was a small problem with the initial formula. T (OptionExcercise) must replace t, except
        /// in the exponential function when t is 0 and T remains the same.
        /// </remarks>
        public double CalculateCallPrice(double StrikePrice, double OptionExcercise)
        {
            this.StrikePrice = StrikePrice;
            double Aux1 = InitialStockPrice * P(0, OptionExcercise, Math.Log(InitialStockPrice));
            double Aux2 = StrikePrice * Math.Exp(-RiskFreeRate * (OptionExcercise));
            double Aux3 = P(1, OptionExcercise, Math.Log(InitialStockPrice));
            double Aux4 = Aux1 - Aux2 * Aux3;
            return Aux4;
        }

        /// <summary>
        /// Formula for the European Put option based on the Put-Call option parity.
        /// </summary>
        /// <returns>
        /// The European Put price of the option.
        /// </returns>
        /// <param name="StrikePrice">The strike price of the option.</param>
        /// <param name="OptionExcercise">The option exercise date of the option.</param>
        public double CalculatePutPrice(double StrikePrice, double OptionExcercise)
        {
            double CallPrice = CalculateCallPrice(StrikePrice, OptionExcercise);
            return CallPrice + StrikePrice * Math.Exp(-RiskFreeRate * OptionExcercise) - InitialStockPrice;
        }

        /// <summary>
        /// Getter for the parameters optimized in task 2.3.
        /// </summary>
        /// <returns>
        /// Array of doubles ordered as follows: Kappa, Theta, Sigma, Rho and Nu.
        /// </returns>
        /// <remarks>
        /// Only for use in calibration.
        /// </remarks>
        public double[] ConvertCalibrationParamsToArray()
        {
            double[] ParamsArray = new double[NumModelParams];
            ParamsArray[0] = Kappa;
            ParamsArray[1] = Theta;
            ParamsArray[2] = Sigma;
            ParamsArray[3] = Rho;
            ParamsArray[4] = Nu;
            return ParamsArray;
        }

    }
}
