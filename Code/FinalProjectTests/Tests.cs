using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FinalProject;
using MathNet.Numerics.Distributions;

namespace FinalProjectTests
{
    /// <summary>
    /// I created this class to test the methods: HestonFormula, HestonMonteCarlo and HestonCalibration. I ran them during the
    /// development to make sure the code was functioning properly.
    /// </summary>
    [TestClass]
    public class HestonTestsFinal
    {
        /// <summary>
        /// The purpose of this test was to see whether HestonMonteCarlo and HestonPrice were producing the same output
        /// for the same parameters.
        /// </summary>
        [TestMethod]
        public void InstanciateHestonFormulaAndMonteCarloAreEqual()
        {
            double RiskFreeRate = 0.1;
            double StrikePrice = 100;
            double OptionExercise = 15;
            double Kappa = 2;
            double Theta = 0.06;
            double Sigma = 0.4;
            double Rho = 0.5;

            double InitialStockPrice = 100;
            double Nu = 0.04;
            int TimeSteps = 365;
            int Paths = 100000;

            double Accuracy = 7;


            double PriceHestonFormula;
            double PriceMonteCarlo;

            HestonFormula Formula = new HestonFormula(RiskFreeRate, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu);
            HestonMonteCarlo MonteCarlo = new HestonMonteCarlo(RiskFreeRate, StrikePrice, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu, TimeSteps);

            PriceHestonFormula = Formula.CalculateCallPrice(StrikePrice, OptionExercise);
            PriceMonteCarlo = MonteCarlo.CalculatePrice(OptionExercise,Paths);
            Assert.AreEqual(PriceHestonFormula, PriceMonteCarlo, Accuracy);



        }

        /// <summary>
        /// The purpose of this test was to see if the calibration was producing a small error.
        /// </summary>
        [TestMethod]
        public void InstanciateHestonCalibrationErrorSmallAndFinisedOK()
        {
            double RiskFreeRate = 0.025;
            double InitialStockPrice = 100;
            double Kappa = 0.5;
            double Theta = 0.01;
            double Sigma = 0.2;
            double Rho = 0.1;
            double Nu = 0.4;
            double Accuracy = 1 / 1000;
            int MaxIterations = 1000;
            double[] StrikePrices = new double[] { 80, 90, 80, 100, 100 };
            double[] OptionExerciseTimes = new double[] { 1, 1, 2, 2, 1.5 };
            double[] Prices = new double[] { 25.72, 18.93, 30.49, 19.36, 16.58 };


            HestonCalibration Calibrator = new HestonCalibration(RiskFreeRate, InitialStockPrice, Accuracy, MaxIterations);

            Calibrator.SetGuessParameters(Kappa, Theta, Sigma, Rho, Nu);

            for (int i = 0; i < Prices.Length; ++i)
            {
                Calibrator.AddObservedOption(OptionExerciseTimes[i], StrikePrices[i], Prices[i]);
            }
            Calibrator.Calibrate();


            double Error = 0;
            FinalProject.CalibrationOutcome Outcome = FinalProject.CalibrationOutcome.NotStarted;
            Calibrator.GetCalibrationStatus(ref Outcome, ref Error);
            Assert.AreEqual(0, Error, 1);


        }





    }
}
    