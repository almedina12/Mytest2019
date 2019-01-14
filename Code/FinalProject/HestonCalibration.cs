using System;
using System.Collections.Generic;


namespace FinalProject
{
    /// <summary>
    /// Exception that will be thrown if the calibration fails.
    /// </summary>
    /// <exception cref="CalibrationFailedException"></exception>
    public class CalibrationFailedException : Exception
    {
        public CalibrationFailedException()
        {
        }
        public CalibrationFailedException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Enumerator used to identify the type of outcome of the calibration.
    /// </summary>
    public enum CalibrationOutcome
    {
        NotStarted,
        FinishedOK,
        FailedMaxItReached,
        FailedOtherReason
    };

    /// <summary>
    /// Struct that contains the information needed to create instances of HestonFormula and 
    /// minimize the squared error.
    /// </summary>
    public struct CallOptionMarketData
    {

        public double OptionExercise;
        public double StrikePrice;
        public double MarketCallPrice;
    }

    /// <summary>
    /// Class used to calibrate the Kappa, Theta, Sigma, Rho and Nu parameters from the HestonFormula class. 
    /// The calibration seeks to reduce the squared error between the model and the observed prices. 
    /// </summary>
    public class HestonCalibration
    {
        private double[] CalibratedParams;
        private readonly double InitialStockPrice;
        private readonly double Accuracy;
        private readonly double RiskFreeRate;
        private readonly int MaxIterations;
        private LinkedList<CallOptionMarketData> MarketOptionsList;
        private CalibrationOutcome Outcome;


        /// <summary>
        /// Constructor for the class.
        /// </summary>
        /// <remarks>
        /// An initial guess is provided but it can be changed.
        /// <see cref="SetGuessParameters(double,double,double,double,double)"/>
        /// </remarks>
        public HestonCalibration(double RiskFreeRate, double InitialStockPrice, double Accuracy, int MaxIterations)
        {
            this.RiskFreeRate = RiskFreeRate;
            this.InitialStockPrice = InitialStockPrice;
            this.Accuracy = Accuracy;
            this.MaxIterations = MaxIterations;
            MarketOptionsList = new LinkedList<CallOptionMarketData>();
            CalibratedParams = new double[] { 0.5, 0.01, 0.2, 0.1, 0.4 };
        }

        /// <summary>
        /// Gives the option to change the initial guess.
        /// </summary>
        /// <param name="Kappa">Double precision parameter.</param>
        /// <param name="Theta">Double precision parameter.</param>
        /// <param name="Sigma">Volatility.</param>
        /// <param name="Rho">Double precision parameter.</param>
        /// <param name="Nu">Initial variance.</param>
        /// <remarks>
        /// By creating an instance of the class HestonFormula we make sure all parameters are fit for purpose.
        /// </remarks>
        public void SetGuessParameters( double Kappa, double Theta, double Sigma, double Rho, double Nu)
        {
            HestonFormula m = new HestonFormula(RiskFreeRate, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu);
            CalibratedParams = m.ConvertCalibrationParamsToArray();
        }

        /// <summary>
        /// Adds an observation that will be used in the calibration.
        /// </summary>
        /// <param name="OptionExercise">Double precision parameter.</param>
        /// <param name="StrikePrice">Double precision parameter.</param>
        /// <param name="MarketCallPrice">Double precision parameter.</param>
        public void AddObservedOption(double OptionExercise, double StrikePrice, double MarketCallPrice)
        {
            CallOptionMarketData ObservedOption;
            ObservedOption.StrikePrice = StrikePrice;
            ObservedOption.OptionExercise = OptionExercise;
            ObservedOption.MarketCallPrice = MarketCallPrice;
            MarketOptionsList.AddLast(ObservedOption);
        }

        /// <summary>
        /// Calculates the difference between the observed and the model prices.
        /// </summary>
        /// <returns>
        /// A double precision number that represents the squared error.
        /// </returns>
        /// <param name="m">Instance of HestonFormula used for the model prices.</param>
        public double CalcMeanSquareErrorBetweenModelAndMarket(HestonFormula m)
        {
            double MeanSqErr = 0;
            foreach (CallOptionMarketData Option in MarketOptionsList)
            {
                double StrikePrice = Option.StrikePrice;
                double OptionExercise = Option.OptionExercise;
                double ModelPrice = m.CalculateCallPrice(StrikePrice, OptionExercise);

                double Difference = ModelPrice - Option.MarketCallPrice;
                MeanSqErr += Difference * Difference;
            }
            return MeanSqErr;
        }

        /// <summary>
        /// Defines the function used by the Alglib minimization algorithm.
        /// </summary>
        /// <param name="ParamsArray">Array of doubles ordered as follows: Kappa, Theta, Sigma, Rho and Nu.</param>
        /// <param name="func">Function used passed by reference.</param>
        /// <param name="obj">Used by Alglib.</param>
        public void CalibrationObjectiveFunction(double[] ParamsArray, ref double func, object obj)
        {
            HestonFormula m = new HestonFormula(RiskFreeRate, InitialStockPrice, ParamsArray);
            func = CalcMeanSquareErrorBetweenModelAndMarket(m);
        }

        /// <summary>
        /// Main function of the class. Uses Alglib to reduce the error between the model and the observed 
        /// prices.
        /// </summary>
        /// <remarks>
        /// <para>Though the function does not return a value, <paramref name="CalibratedParams"/> contains
        /// the result if there was not an exception.</para>
        /// <para>The value of <paramref name="Stpmax"/> is very important for the convergence of the algorithm.</para>
        /// <para>Epsg, Epsf, and Epsx are taken equal.</para>
        /// <para>The method prints out the result in the console.</para>
        /// </remarks>
        /// <exception cref="CalibrationFailedException">
        /// Thrown if the outcome type is not either- 1,2,4 or 5, i.e it did not converge or reach the maximum
        /// number of iterations.
        /// </exception>
        public void Calibrate()
        {
            Outcome = CalibrationOutcome.NotStarted;
            double[] InitialParams = new double[5];
            CalibratedParams.CopyTo(InitialParams, 0); 
            double Epsg = Accuracy;
            double Epsf = Accuracy; 
            double Epsx = Accuracy;
            double Diffstep = 1.0e-6;
            int Maxits = MaxIterations;
            double Stpmax = 0.05;

            alglib.minlbfgscreatef(5, InitialParams, Diffstep, out alglib.minlbfgsstate State);
            alglib.minlbfgssetcond(State, Epsg, Epsf, Epsx, Maxits);
            alglib.minlbfgssetstpmax(State, Stpmax);

            alglib.minlbfgsoptimize(State, CalibrationObjectiveFunction, null, null);

            double[] ResultParams = new double[5];
            alglib.minlbfgsresults(State, out ResultParams, out alglib.minlbfgsreport Rep);

            System.Console.WriteLine("Termination type: {0}", Rep.terminationtype);
            System.Console.WriteLine("Num iterations {0}", Rep.iterationscount);
            System.Console.WriteLine("{0}", alglib.ap.format(ResultParams, 5));

            if (Rep.terminationtype == 1	
                || Rep.terminationtype == 2
                || Rep.terminationtype == 4)
            {    	
                Outcome = CalibrationOutcome.FinishedOK;

                CalibratedParams = ResultParams;
            }
            else if (Rep.terminationtype == 5)
            {	
                Outcome = CalibrationOutcome.FailedMaxItReached;

                CalibratedParams = ResultParams;

            }
            else
            {
                Outcome = CalibrationOutcome.FailedOtherReason;
                throw new CalibrationFailedException("-- Model calibration failed badly. --");
            }
        }

        /// <summary>
        /// Used to know the outcome and error of the calibration.
        /// </summary>
        /// <param name="CalibOutcome">Must be a CalibrationOutcome object. It is modified by reference.</param>
        /// <param name="PricingError">Represents the error. It is modified by reference.</param>
        public void GetCalibrationStatus(ref CalibrationOutcome CalibOutcome, ref double PricingError)
        {
            CalibOutcome = Outcome;
            HestonFormula m = new HestonFormula(RiskFreeRate, InitialStockPrice, CalibratedParams);
            PricingError = CalcMeanSquareErrorBetweenModelAndMarket(m);
        }

        /// <summary>
        /// A way to get all the parameters of the model.
        /// </summary>
        /// <returns>
        /// A HestonFormula object with all the information of the model.
        /// </returns>
        public HestonFormula GetCalibratedModel()
        {
            HestonFormula m = new HestonFormula(RiskFreeRate, InitialStockPrice, CalibratedParams);
            return m;
        }
    }

}


