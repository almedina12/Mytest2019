using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HestonModel;
using HestonModel.Interfaces;
using HestonModel.TheClasses;

namespace HestonCmdLine
{
    /// <summary>
    /// This class exists to test whether the set up was done correctly in the HestonModel project. It calls all its functions
    /// and prints out the results.
    /// </summary>
    public class CorrectSetUp
    {
        /// <summary>
        /// This function tests the first function of the Heston class and prints the results.
        /// </summary>
        public static void CalibrationSetUp()
        {
            double RiskFreeRate = 0.025;
            double InitialStockPrice = 100;
            double Kappa = 0.5;
            double Theta = 0.01;
            double Sigma = 0.2;
            double Rho = 0.1;
            double Nu = 0.4;
            double Accuracy = 0.001;
            int MaxIterations = 1000;


            double[] StrikePrices = new double[] { 80, 90, 80, 100, 100 };
            double[] OptionExerciseTimes = new double[] { 1, 1, 2, 2, 1.5 };
            double[] Prices = new double[] { 25.72, 18.93, 30.49, 19.36, 16.58 };

            //These are implementation of interfaces that are necessary to interact with the HestonModel project.
            EuropeanOption TheOption;
            OptionMarketData Data;
            HestonEnumerable2 Enum_ = new HestonEnumerable2();


            for (int i = 0; i < StrikePrices.Length; i++)
            {
                TheOption = new EuropeanOption(StrikePrices[i], OptionExerciseTimes[i], HestonModel.PayoffType.Call);
                Data = new OptionMarketData(TheOption, Prices[i]);
                Enum_[i] = Data;
                
            }

            VarianceProcessParameters VProcessParameters = new VarianceProcessParameters(Kappa, Theta, Sigma, Nu, Rho);
            HestonModelParameters guessModelParameters = new HestonModelParameters(InitialStockPrice, RiskFreeRate, VProcessParameters);
            CalibrationSettings Settings = new CalibrationSettings(Accuracy, MaxIterations);

            HestonCalibrationResult Result = Heston.CalibrateHestonParameters(guessModelParameters,Enum_, Settings);

            Console.WriteLine(Result.Parameters.VarianceParameters.Kappa);
            Console.WriteLine(Result.Parameters.VarianceParameters.Rho);
            Console.WriteLine(Result.Parameters.VarianceParameters.Sigma);
            Console.WriteLine(Result.Parameters.VarianceParameters.Theta);
            Console.WriteLine(Result.Parameters.VarianceParameters.V0);
            Console.WriteLine(Result.PricingError);
            Console.ReadKey();
        }
        /// <summary>
        /// This functions tests the second function of the class Heston and prints out the results.
        /// </summary>
        public static void FormulaSetUp()
        {
            double RiskFreeRate = 0.025;
            double StrikePrice = 100;
            double[] OptionExercise = { 1, 2, 3, 4, 15 };
            double Kappa = 1.5768;
            double Theta = 0.0398;
            double Sigma = 0.5751;
            double Rho = -0.5711;
            double InitialStockPrice = 100;
            double Nu = 0.0175;
            VarianceProcessParameters VProcessParameters = new VarianceProcessParameters(Kappa, Theta, Sigma, Nu, Rho);
            EuropeanOption europeanOption = new EuropeanOption(StrikePrice, OptionExercise[4], HestonModel.PayoffType.Call);
            HestonModelParameters parameters = new HestonModelParameters(InitialStockPrice, RiskFreeRate, VProcessParameters);
            double Price = Heston.HestonEuropeanOptionPrice(parameters, europeanOption);
            Console.WriteLine(Price);
            Console.ReadKey();
        }
        /// <summary>
        /// This function tests the third function in the Heston class and prints out the results.
        /// </summary>
        public static void MonteCarloSetUp()
        {
            double RiskFreeRate = 0.1;
            double StrikePrice = 100;
            double[] OptionExercise = { 1, 2, 3, 4, 15 };
            double Kappa = 2;
            double Theta = 0.06;
            double Sigma = 0.4;
            double Rho = 0.5;
            double InitialStockPrice = 100;
            double Nu = 0.04;
            int TimeSteps = 1000;
            int Paths = 10000;
            double Price= 0;
            for(int i = 0; i < OptionExercise.Length; i++)
            {
                VarianceProcessParameters VProcessParameters = new VarianceProcessParameters(Kappa, Theta, Sigma, Nu, Rho);
                HestonModelParameters parameters = new HestonModelParameters(InitialStockPrice, RiskFreeRate, VProcessParameters);
                EuropeanOption europeanOption = new EuropeanOption(StrikePrice, OptionExercise[i], HestonModel.PayoffType.Call);
                MonteCarloSettings settings = new MonteCarloSettings(Paths, TimeSteps);
                Price = Heston.HestonEuropeanOptionPriceMC(parameters, europeanOption, settings);
                Console.WriteLine(Price);
            }

            Console.ReadKey();
        }
        /// <summary>
        /// This function tests the fourth function in the Heston class and prints out the results.
        /// </summary>
        public static void AsianOptionsSetUp()
        {
            double RiskFreeRate = 0.1;
            double StrikePrice = 100;
            double[] OptionExercise = { 1, 2, 3 };
            double[] Aux1 = { 0.75, 1 };
            double[] Aux2 = { 0.25, 0.5, 0.75, 1, 1.25, 1.5, 1.75 };
            double[] Aux3 = { 1, 2, 3 };
            double[][] Dates = { Aux1, Aux2, Aux3 };
            double Kappa = 2;
            double Theta = 0.06;
            double Sigma = 0.4;
            double Rho = 0.5;
            double InitialStockPrice = 100;
            double Nu = 0.04;
            int TimeSteps = 365;
            int Paths = 10000;

            

            for (int i = 0; i < OptionExercise.Length; i++)
            {
                HestonEnumerable Enume = new HestonEnumerable(Dates[i]);
                VarianceProcessParameters VProcessParameters = new VarianceProcessParameters(Kappa, Theta, Sigma, Nu, Rho);
                HestonModelParameters parameters = new HestonModelParameters(InitialStockPrice, RiskFreeRate, VProcessParameters);
                AsianOption Option = new AsianOption(StrikePrice, OptionExercise[i], Enume, PayoffType.Call);
                MonteCarloSettings settings = new MonteCarloSettings(Paths, TimeSteps);
                double Price = Heston.HestonAsianOptionPriceMC(parameters, Option, settings);
                Console.WriteLine(Price);
            }

            Console.ReadKey();

        }
        /// <summary>
        /// This function tests the fifth function in the Heston class and prints out the results.
        /// </summary>
        public static void LookbackOptionSetUp()
        {
            double RiskFreeRate = 0.1;
            double[] OptionExercise = { 1, 3, 5, 7, 9 };
            double Kappa = 2;
            double Theta = 0.06;
            double Sigma = 0.4;
            double Rho = 0.5;
            double InitialStockPrice = 100;
            double Nu = 0.04;
            int TimeSteps = 365;
            int Paths = 10000;
            for (int i=0; i<OptionExercise.Length;i++)
            {
                VarianceProcessParameters VProcessParameters = new VarianceProcessParameters(Kappa, Theta, Sigma, Nu, Rho);
                HestonModelParameters parameters = new HestonModelParameters(InitialStockPrice, RiskFreeRate, VProcessParameters);
                EuropeanOption Option = new EuropeanOption(0, OptionExercise[i], HestonModel.PayoffType.Call);
                MonteCarloSettings settings = new MonteCarloSettings(Paths, TimeSteps);
                double Price = Heston.HestonLookbackOptionPriceMC(parameters, Option, settings);
                Console.WriteLine(Price);
            }
            Console.ReadKey();
        }

    }
}


