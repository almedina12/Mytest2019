using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;
using FinalProject;

namespace HestonCmdLine
{
    /// <summary>
    /// This class was created to experiment with the HestonCalibration Class to answer task 2.6. 
    /// It is in the Console Application so that the output can be seen.
    /// </summary>
    class CheckingCalibration
    {
        /// <summary>
        /// This is the only method of the class. It initializes the data, the calibrator, calls the calibration function
        /// and prints out all the results. This include the calibration outcome, error, Kappa, Theta, Sigma, Rho, Nu
        /// and the approximated prices.
        /// </summary>
        public static void CheckCalibration()
        {
            double Error = 0;

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

            Console.WriteLine("RiskFreeRate: {0} -- InitialStockPrice: {1} -- Kappa: {2} -- Theta {3} -- Sigma {4} -- Rho {5} -- Nu {6}"
             , RiskFreeRate, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu);

            HestonCalibration Calibrator = new HestonCalibration(RiskFreeRate, InitialStockPrice, Accuracy, MaxIterations);

            Calibrator.SetGuessParameters(Kappa, Theta, Sigma, Rho, Nu);

            for (int i = 0; i < Prices.Length; ++i)
            {
                Calibrator.AddObservedOption(OptionExerciseTimes[i], StrikePrices[i], Prices[i]);
            }
            Calibrator.Calibrate();
            CalibrationOutcome Outcome = CalibrationOutcome.NotStarted;
            Calibrator.GetCalibrationStatus(ref Outcome, ref Error);

            HestonFormula CalibratedModel = Calibrator.GetCalibratedModel();
            double[] Param = CalibratedModel.ConvertCalibrationParamsToArray();
            double Price;

            Console.WriteLine("RiskFreeRate: {0} -- InitialStockPrice: {1} -- Kappa: {2} -- Theta {3} -- Sigma {4} -- Rho {5} -- Nu {6}"
                , RiskFreeRate, InitialStockPrice, Param[0], Param[1], Param[2], Param[3], Param[4]);
            for (int i = 0; i < Prices.Length; i++)
            {
                Price = CalibratedModel.CalculateCallPrice(StrikePrices[i], OptionExerciseTimes[i]);
                Console.WriteLine(" {3} -- Strike {0} -- Exercise {1} -- Price {2}", StrikePrices[i], OptionExerciseTimes[i], Price, i);
            }
            Console.WriteLine("Calibration outcome: {0} and error: {1}", Outcome, Error);





            Console.ReadKey();
        }


        
    }
}
