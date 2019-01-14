using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FinalProject;

namespace FinalProjectTests
{
    /// <summary>
    /// The purpose of this class is to provide the functions that will be used to plot the error between the prices generated
    /// by the Monte Carlo and the Heston formula algorithms.
    /// </summary>
    public class ConvergenceHestonMonteCarlo
    {
        private readonly double RiskFreeRate;
        private readonly double StrikePrice;
        private readonly double OptionExercise;
        private readonly double Kappa;
        private readonly double Theta;
        private readonly double Sigma;
        private readonly double Rho;
        private readonly double InitialStockPrice;
        private readonly double Nu;

        /// <summary>
        /// Constructor for the class. All values can be set by default.
        /// </summary>
        /// <param name="RiskFreeRate"></param>
        /// <param name="StrikePrice"></param>
        /// <param name="OptionExercise"></param>
        /// <param name="Kappa"></param>
        /// <param name="Theta"></param>
        /// <param name="Sigma"></param>
        /// <param name="Rho"></param>
        /// <param name="InitialStockPrice"></param>
        /// <param name="Nu"></param>
        /// <remarks>We used the same data provided in task 2.3.</remarks>
        public ConvergenceHestonMonteCarlo(
            double RiskFreeRate = 0.1,
            double StrikePrice = 100,
            double OptionExercise = 1,
            double Kappa = 2,
            double Theta = 0.06,
            double Sigma = 0.4,
            double Rho = 0.5,
            double InitialStockPrice = 100,
            double Nu = 0.04)
        {
            this.RiskFreeRate = RiskFreeRate;
            this.StrikePrice = StrikePrice;
            this.OptionExercise = OptionExercise;
            this.Kappa = Kappa;
            this.Theta = Theta;
            this.Sigma = Sigma;
            this.Rho = Rho;
            this.InitialStockPrice = InitialStockPrice;
            this.Nu = Nu;

        }
        /// <summary>
        /// Given an array of paths it calculates the error for each element of the array.
        /// </summary>
        /// <param name="X">The array of paths.</param>
        /// <param name="TimeSteps"></param>
        /// <returns>An array that represents the error between the HestonMonteCarlo price and the HestonFormula price.</returns>
        public double[] DataPointsPath(int[] X, int TimeSteps = 356)
        {
            double[] Y = new double[X.Length];

            for (int i=1; i<X.Length; i++)
            {
                Y[i] = CalculateError(X[i], TimeSteps);
            }
            return Y;

        }
        /// <summary>
        /// Given an array of time steps it calculates the error for each element of the array.
        /// </summary>
        /// <param name="X">the array of time steps</param>
        /// <param name="Paths"></param>
        /// <returns>An array that represents the error between the HestonMonteCarlo price and the HestonFormula price.</returns>
        public double[] DataPointsTimeSteps(int[] X, int Paths = 10000)
        {
            double[] Y = new double[X.Length];

            for (int i = 1; i < X.Length; i++)
            {
                Y[i] = CalculateError(Paths, X[i]);
            }
            return Y;

        }

        /// <summary>
        /// Calculates the error for a single instance of the HestonMonteCarlo and the HestonFormula.
        /// </summary>
        /// <param name="Paths"></param>
        /// <param name="TimeSteps"></param>
        /// <returns>The error for the price of the HestonMonteCarlo and the HestonFormula.</returns>
        public double CalculateError(int Paths, int TimeSteps)
        {
            HestonFormula Formula = new HestonFormula(RiskFreeRate, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu);
            HestonMonteCarlo MonteCarlo = new HestonMonteCarlo(RiskFreeRate, StrikePrice, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu, TimeSteps);
            double PriceHestonFormula = Formula.CalculateCallPrice(StrikePrice, OptionExercise);
            double PriceMonteCarlo = MonteCarlo.CalculatePrice(OptionExercise, Paths);
            return Math.Abs(PriceHestonFormula - PriceMonteCarlo);
        }


    }
}
