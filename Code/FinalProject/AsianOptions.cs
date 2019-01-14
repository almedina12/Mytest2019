using System;
using System.Collections;

namespace FinalProject
{

    /// <summary>
    /// Subclass of HestonMonteCarlo. Seeks to find the approximated price of Call and Put Asian type options
    /// relaying on the methods from HestonMonteCarlo.
    /// </summary>
    public class AsianOptions: HestonMonteCarlo {

        protected internal IEnumerable Dates;
        protected internal bool IsCall = true;

        public AsianOptions(IEnumerable Dates,
            double RiskFreeRate, 
            double StrikePrice, 
            double InitialStockPrice, 
            double Kappa, 
            double Theta, 
            double Sigma, 
            double Rho, 
            double Nu, 
            int TimeSteps) : 
            base(RiskFreeRate, 
                StrikePrice, 
                InitialStockPrice, 
                Kappa, 
                Theta, 
                Sigma, 
                Rho, 
                Nu, 
                TimeSteps)
        {
            this.Dates = Dates;
        }

        /// <summary>
        /// Payoff of the option for Asian Call Options. Overrides the method from the parent class.
        /// </summary>
        /// <returns>
        /// Double precision number.
        /// </returns>
        /// <param name="Path"> Array of doubles that represents the path of the underlaying asset.</param>
        protected override double Payoff(double[] Path)
        {
            double Sum = 0;
            double Cont = 0;

            foreach(double T in Dates)
            {
                Sum = Sum + Path[(int)Math.Floor(T * TimeSteps - 1)];
                Cont++;

            }
            Sum = Sum / Cont;

            double Result;

            if (IsCall)
                Result = Math.Max(0, Sum - StrikePrice);
            else
                Result = Math.Max(0, StrikePrice - Sum);


            return Result;
        }

        /// <summary>
        /// Payoff of the option for Asian Put Options. Overrides the method from the parent class.
        /// </summary>
        /// <returns>
        /// Double precision number.
        /// </returns>
        /// <param name="NPaths"> Number of path that will be created.</param>
        /// <param name="OptionExercise"> The maturity/exercise date of the option..</param>
        public override double CalculatePutPrice(double OptionExercise, int NPaths = 10000)
        {
            IsCall = false;
            double Result = CalculatePrice(OptionExercise, NPaths);
            IsCall = true;
            return Result;
        }




    }




}
