using System;
using FinalProject;


namespace HestonCmdLine
{

    /// <summary>
    /// This is the class I used to expose most of the functionality I implemented through the console App.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// This function creates an instance of HestonFormula and prints out the price of several European Call Options.
        /// </summary>
        public static void HestonFormulaResult()
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


            HestonFormula Formula = new HestonFormula(RiskFreeRate, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu);

            for (int h = 0; h < OptionExercise.Length; h++)
            {
                double HestonFormula = Formula.CalculateCallPrice(StrikePrice, OptionExercise[h]);

                Console.WriteLine(HestonFormula);

            }
            Console.ReadKey();
        }

        /// <summary>
        /// This function creates an instance of HestonMonteCarlo and calculates the price for several oEuropean Call Options.
        /// </summary>
        public static void HestonMonteCarloResult()
        {
            Console.WriteLine("Monte Carlo:");
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


            for (int h = 0; h < OptionExercise.Length; h++)
            {

                HestonMonteCarlo MonteCarlo = new HestonMonteCarlo(RiskFreeRate, StrikePrice, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu, TimeSteps);
                double Price = MonteCarlo.CalculatePrice(OptionExercise[h], Paths);
                Console.WriteLine(Price);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// This function creates an instance of the class AsianOptions and prints out the Call price for various sets of dates.
        /// </summary>
        public static void AsianOptionsResult()
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


            for (int h = 0; h < Dates.Length; h++)
            {

                AsianOptions Asian = new AsianOptions(Dates[h],RiskFreeRate, StrikePrice, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu, TimeSteps);
                double Price = Asian.CalculatePrice(OptionExercise[h],Paths);
                Console.WriteLine(Price);
            }


            Console.ReadKey();

        }

        /// <summary>
        /// This function creates an instance of the class LookbackOptions and prints out the price for various maturities.
        /// </summary>
        public static void LookbackOptionsResult()
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


            for (int h = 0; h < OptionExercise.Length; h++)
            {

                LookbackOptions Asian = new LookbackOptions( RiskFreeRate, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu, TimeSteps);
                double Price = Asian.CalculatePrice(OptionExercise[h], Paths);
                Console.WriteLine(Price);
            }


            Console.ReadKey();

        }
        /// <summary>
        /// This function creates an instance of CliquetOption and prints out the price for a set of fixing dates.
        /// </summary>
        public static void CliquetOptionResult()
        {
            double RiskFreeRate = 0.1;
            double[] FixingDates = { 1, 2, 3};
            int Lambda = 1;
            double StrikePrice = 90;
            double InitialStockPrice = 100;
            double PayoutRate = 0.05;
            double Sigma = 0.2;


            CliquetOptions Option = new CliquetOptions(RiskFreeRate, FixingDates, Lambda, Sigma, StrikePrice, InitialStockPrice, PayoutRate);
            double Result = Option.CalculatePrice();
            Console.WriteLine(Result);
            Console.ReadKey();


        }
        /// <summary>
        /// This function creates an instance of EverestOptions using data from several different assets. It prints out the price
        /// of the option.
        /// </summary>
        public static void EverestOptionsResult()
        {
            double RiskFreeRate = 0.1;
            double[] StrikePrice = { 100, 110, 120, 100, 110, 120 };
            double OptionExercise = 15;

            double[] Kappa = { 2, 2.5, 3.5, 2, 2.5, 3.5 };
            double[] Theta = { 0.6, 0.6, 0.7, 0.6, 0.6, 0.7 };
            double[] Sigma = { 0.4, 0.3, 0.02, 0.4, 0.3, 0.02 }; 
            double[] Rho = { 0.7, 0.12, 0.394, 0.7, 0.12, 0.394 }; 

            double[] InitialStockPrice = { 98, 100, 115, 98, 100, 115 };
            double[] Nu = { 0.07, 0.02, 0.09, 0.07, 0.02, 0.09 };


            EverestOptions Everest = new EverestOptions(RiskFreeRate);

            for (int h = 0; h < StrikePrice.Length; h++)
                Everest.AddAsset(StrikePrice[h], InitialStockPrice[h], Kappa[h], Theta[h], Sigma[h], Rho[h], Nu[h]);

            double Price = Everest.CalculatePrice(OptionExercise);
            Console.WriteLine(Price);
            Console.ReadKey();

        }



    }
}
