using System;
using System.Collections.Generic;
using HestonModel.Interfaces;
using FinalProject;
using System.Linq;

namespace HestonModel
{

    /// <summary> 
    /// This class will be used for grading. 
    /// Don't remove any of the methods and don't modify their signatures. Don't change the namespace. 
    /// Your code should be implemented in other classes (or even projects if you wish), and the relevant functionality should only be called here and outputs returned.
    /// You don't need to implement the interfaces that have been provided if you don't want to.
    /// </summary>
    public static class Heston
    {
        /// <summary>
        /// Method for calibrating the heston model.
        /// </summary>
        /// <param name="guessModelParameters">Object implementing IHestonModelParameters interface containing the risk-free rate, initial stock price
        /// and initial guess parameters to be used in the calibration.</param>
        /// <param name="referenceData">A collection of objects implementing IOptionMarketData<IEuropeanOption> interface. These should contain the reference data used for calibration.</param>
        /// <param name="calibrationSettings">An object implementing ICalibrationSettings interface.</param>
        /// <returns>Object implementing IHestonCalibrationResult interface which contains calibrated model parameters and additional diagnostic information</returns>
        public static HestonCalibrationResult CalibrateHestonParameters(IHestonModelParameters guessModelParameters,
            IEnumerable<IOptionMarketData<IEuropeanOption>> referenceData, ICalibrationSettings calibrationSettings)
        {
            double RiskFreeRate = guessModelParameters.RiskFreeRate;
            double InitialStockPrice = guessModelParameters.InitialStockPrice;
            double Kappa = guessModelParameters.VarianceParameters.Kappa;
            double Theta = guessModelParameters.VarianceParameters.Theta;
            double Sigma = guessModelParameters.VarianceParameters.Sigma;
            double Rho = guessModelParameters.VarianceParameters.Rho;
            double Nu = guessModelParameters.VarianceParameters.V0;
            double Accuracy = calibrationSettings.Accuracy;
            int MaxIterations = calibrationSettings.MaximumNumberOfIterations;
            int j = 0;

            double[] StrikePrices = new double[referenceData.Count()];
            double[] OptionExerciseTimes = new double[referenceData.Count()];
            double[] Prices = new double[referenceData.Count()];

            foreach (IOptionMarketData<IEuropeanOption> Data in referenceData)
            {
                StrikePrices[j] = Data.Option.StrikePrice;
                OptionExerciseTimes[j] = Data.Option.Maturity;
                Prices[j] = Data.Price;
                j++;
            }

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
            HestonFormula CalibratedModel = Calibrator.GetCalibratedModel();
            double[] Param = CalibratedModel.ConvertCalibrationParamsToArray();
            VarianceProcessParameters VParam = new VarianceProcessParameters(Param[0], Param[1], Param[2], Param[4], Param[3]);
            HestonModelParameters HestonMP = new HestonModelParameters(InitialStockPrice, RiskFreeRate, VParam);
            HestonCalibrationResult Result;

            if (Outcome == FinalProject.CalibrationOutcome.FailedOtherReason)
            {
                Result = new HestonCalibrationResult(Error, HestonMP, CalibrationOutcome.FailedOtherReason);
            }
            else if (Outcome == FinalProject.CalibrationOutcome.FailedMaxItReached)
            {
                Result = new HestonCalibrationResult(Error, HestonMP, CalibrationOutcome.FailedMaxItReached);
            }
            else if (Outcome == FinalProject.CalibrationOutcome.NotStarted)
            {
                Result = new HestonCalibrationResult(Error, HestonMP, CalibrationOutcome.NotStarted);
            }
            else
            {
                Result = new HestonCalibrationResult(Error, HestonMP, CalibrationOutcome.FinishedOK);
            }

            return Result;

        }

        /// <summary>
        /// Price a European option in the Heston model using the Heston formula. This should be accurate to 5 decimal places
        /// </summary>
        /// <param name="parameters">Object implementing IHestonModelParameters interface, containing model parameters.</param>
        /// <param name="europeanOption">Object implementing IEuropeanOption interface, containing the option parameters.</param>
        /// <returns>Option price</returns>
        public static double HestonEuropeanOptionPrice(IHestonModelParameters parameters, IEuropeanOption europeanOption)
        {
            double RiskFreeRate = parameters.RiskFreeRate;
            double StrikePrice = europeanOption.StrikePrice;
            double OptionExercise = europeanOption.Maturity;
            double Kappa = parameters.VarianceParameters.Kappa;
            double Theta = parameters.VarianceParameters.Theta;
            double Sigma = parameters.VarianceParameters.Sigma;
            double Rho = parameters.VarianceParameters.Rho;
            double InitialStockPrice = parameters.InitialStockPrice;
            double Nu = parameters.VarianceParameters.V0;

            HestonFormula Formula = new HestonFormula(RiskFreeRate, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu);

            double Price = 0;

            if (europeanOption.Type == PayoffType.Call)
            {
                Price = Formula.CalculateCallPrice(StrikePrice, OptionExercise);
            }
            else if (europeanOption.Type == PayoffType.Put)
            {
                Price = Formula.CalculatePutPrice(StrikePrice, OptionExercise);
            }

            return Price;

        }

        /// <summary>
        /// Price a European option in the Heston model using the Monte-Carlo method. Accuracy will depend on number of time steps and samples
        /// </summary>
        /// <param name="parameters">Object implementing IHestonModelParameters interface, containing model parameters.</param>
        /// <param name="europeanOption">Object implementing IEuropeanOption interface, containing the option parameters.</param>
        /// <param name="monteCarloSimulationSettings">An object implementing IMonteCarloSettings object and containing simulation settings.</param>
        /// <returns>Option price</returns>
        public static double HestonEuropeanOptionPriceMC(IHestonModelParameters parameters, IEuropeanOption europeanOption, IMonteCarloSettings monteCarloSimulationSettings)
        {

            double RiskFreeRate = parameters.RiskFreeRate;
            double StrikePrice = europeanOption.StrikePrice;
            double OptionExercise = europeanOption.Maturity;
            double Kappa = parameters.VarianceParameters.Kappa;
            double Theta = parameters.VarianceParameters.Theta;
            double Sigma = parameters.VarianceParameters.Sigma;
            double Rho = parameters.VarianceParameters.Rho;
            double InitialStockPrice = parameters.InitialStockPrice;
            double Nu = parameters.VarianceParameters.V0;
            int TimeSteps = monteCarloSimulationSettings.NumberOfTimeSteps;
            int Paths = monteCarloSimulationSettings.NumberOfTrials;

            HestonMonteCarlo MonteCarlo = new HestonMonteCarlo(RiskFreeRate, StrikePrice, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu, TimeSteps);

            double Price = 0;

            if (europeanOption.Type == PayoffType.Call)
            {
                Price = MonteCarlo.CalculatePrice(OptionExercise, Paths);
            }
            else if (europeanOption.Type == PayoffType.Put)
            {
                Price = MonteCarlo.CalculatePutPrice(OptionExercise, Paths);
            }

            return Price;
        }

        /// <summary>
        /// Price a Asian option in the Heston model using the 
        /// Monte-Carlo method. Accuracy will depend on number of time steps and samples</summary>
        /// <param name="parameters">Object implementing IHestonModelParameters interface, containing model parameters.</param>
        /// <param name="asianOption">Object implementing IAsian interface, containing the option parameters.</param>
        /// <param name="monteCarloSimulationSettings">An object implementing IMonteCarloSettings object and containing simulation settings.</param>
        /// <returns>Option price</returns>
        public static double HestonAsianOptionPriceMC(IHestonModelParameters parameters, IAsianOption asianOption, IMonteCarloSettings monteCarloSimulationSettings)
        {
            IEnumerable<double> Dates = asianOption.MonitoringTimes;
            double RiskFreeRate = parameters.RiskFreeRate;
            double StrikePrice = asianOption.StrikePrice;
            double OptionExercise = asianOption.Maturity;
            double Kappa = parameters.VarianceParameters.Kappa;
            double Theta = parameters.VarianceParameters.Theta;
            double Sigma = parameters.VarianceParameters.Sigma;
            double Rho = parameters.VarianceParameters.Rho;
            double InitialStockPrice = parameters.InitialStockPrice;
            double Nu = parameters.VarianceParameters.V0;
            int TimeSteps = monteCarloSimulationSettings.NumberOfTimeSteps;
            int Paths = monteCarloSimulationSettings.NumberOfTrials;

            AsianOptions AsianOption = new AsianOptions(Dates, RiskFreeRate, StrikePrice, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu, TimeSteps);

            double Price = 0;

            if (asianOption.Type == PayoffType.Call)
            {
                Price = AsianOption.CalculatePrice(OptionExercise, Paths);
            }
            else if (asianOption.Type == PayoffType.Put)
            {
                Price = AsianOption.CalculatePutPrice(OptionExercise, Paths);
            }

            return Price;
        }

        /// <summary>
        /// Price a lookback option in the Heston model using the  
        /// a Monte-Carlo method. Accuracy will depend on number of time steps and samples </summary>
        /// <param name="parameters">Object implementing IHestonModelParameters interface, containing model parameters.</param>
        /// <param name="maturity">An object implementing IOption interface and containing option's maturity</param>
        /// <param name="monteCarloSimulationSettings">An object implementing IMonteCarloSettings object and containing simulation settings.</param>
        /// <returns>Option price</returns>
        public static double HestonLookbackOptionPriceMC(IHestonModelParameters parameters, IOption maturity, IMonteCarloSettings monteCarloSimulationSettings)
        {
            double RiskFreeRate = parameters.RiskFreeRate;

            double OptionExercise = maturity.Maturity;
            double Kappa = parameters.VarianceParameters.Kappa;
            double Theta = parameters.VarianceParameters.Theta;
            double Sigma = parameters.VarianceParameters.Sigma;
            double Rho = parameters.VarianceParameters.Rho;
            double InitialStockPrice = parameters.InitialStockPrice;
            double Nu = parameters.VarianceParameters.V0;
            int TimeSteps = monteCarloSimulationSettings.NumberOfTimeSteps;
            int Paths = monteCarloSimulationSettings.NumberOfTrials;

            LookbackOptions Lookback = new LookbackOptions(RiskFreeRate, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu, TimeSteps);

            double Price = Lookback.CalculatePrice(OptionExercise, Paths);

            return Price;
        }
    }
}
