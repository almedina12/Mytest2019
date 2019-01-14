using System;
using System.Linq;


namespace FinalProject
{
    /// <summary>
    /// Subclass of HestonMonteCarlo. Seeks to find the approximated price of Lookback type options
    /// relaying on the methods from HestonMonteCarlo. The only major change is the Payoff function.
    /// </summary>
    /// <remarks>
    /// The strike price is irrelevant for the pricing of Lookback options.
    /// </remarks>
    public class LookbackOptions : HestonMonteCarlo
    {
        public LookbackOptions(
            double RiskFreeRate, 
            double InitialStockPrice, 
            double Kappa, 
            double Theta, 
            double Sigma, 
            double Rho, 
            double Nu, 
            int TimeSteps) : 
            base(RiskFreeRate, 
                1, 
                InitialStockPrice, 
                Kappa, 
                Theta, 
                Sigma, 
                Rho, 
                Nu, 
                TimeSteps)
        {
        }

        /// <summary>
        /// Payoff of the option for the Lookback Options. Overrides the method from the parent class.
        /// </summary>
        /// <returns>
        /// Double precision number.
        /// </returns>
        /// <param name="Path"> Array of doubles that represent the path of the underlaying asset.</param>
        protected override double Payoff(double[] Path)
        {
            return Math.Max(0, Path[Path.Length - 1] - Path.Min());
        }

        /// <summary>
        /// Since there is no differentiation between Call and Puts for Lookback Options it calculates the same price for both.
        /// </summary>
        /// <param name="OptionExercise"></param>
        /// <param name="NPaths"></param>
        /// <returns>The Same price as CalculatePrice </returns>
        public override double CalculatePutPrice(double OptionExercise, int NPaths = 10000)
        {
            return CalculatePrice(OptionExercise, NPaths);
        }


    }
}
