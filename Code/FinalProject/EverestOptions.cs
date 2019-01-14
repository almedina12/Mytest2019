using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FinalProject
{
    public class NoAssetsAddedException : Exception
    {
        /// <summary>
        /// Exception that will be thrown if one attempts to calculate the price and the portfolio has no assets.
        /// </summary>
        public NoAssetsAddedException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Class for pricing Everest Options.
    /// An Everest option is a type of exotic option belonging to a class known as mountain range 
    /// options. The value of an Everest option is based on a basket of underlying securities.
    /// </summary>
    public class EverestOptions
    {
        private List<HestonMonteCarlo> Assets = new List<HestonMonteCarlo>();
        private readonly int TimeSteps;
        private readonly double RiskFreeRate;

        public EverestOptions(double RiskFreeRate, int TimeSteps =365)
        {
            this.RiskFreeRate = RiskFreeRate;
            this.TimeSteps = TimeSteps;
        }

        /// <summary>
        /// Adds an asset to the portfolio that will be used in the option price calculation.
        /// </summary>
        /// <param name="StrikePrice"></param>
        /// <param name="InitialStockPrice"></param>
        /// <param name="Kappa"></param>
        /// <param name="Theta"></param>
        /// <param name="Sigma"></param>
        /// <param name="Rho"></param>
        /// <param name="Nu"></param>
        public void AddAsset(
            double StrikePrice,
            double InitialStockPrice,
            double Kappa,
            double Theta,
            double Sigma,
            double Rho,
            double Nu)
        {

            HestonMonteCarlo NewAsset = new HestonMonteCarlo(RiskFreeRate, StrikePrice, InitialStockPrice, Kappa,
                Theta, Sigma, Rho, Nu, TimeSteps);
            Assets.Add(NewAsset);
        }

        private double AuxFunction(double [] Path)
        {
            return Path[Path.Length - 1] / Path[0];
        }
        /// <summary>
        /// Calculate the price of an Everest Option. You must have added assets for it to work.
        /// </summary>
        /// <param name="OptionExercise"></param>
        /// <param name="NPaths"></param>
        /// <returns>The Price.</returns>
        /// <remarks>
        /// We use multi-threading just is we did in HestonMonteCarlo.
        /// </remarks>
        /// <exception cref="NoAssetsAddedException">
        /// Thrown we one attempts to calculate the price when the portfolio has no assets.
        /// </exception>
        public double CalculatePrice(double OptionExercise, int NPaths = 10000)
        {
            if (Assets is null)
                throw new NoAssetsAddedException("--- You have added no assets to the models. ---");

            int NumSamples = (int)Math.Ceiling(OptionExercise * TimeSteps);
            double Tau = OptionExercise / NumSamples;

            double Aux = 0;
            double[] Aux2 = new double[Assets.Count];
            Parallel.For(0, NPaths, i => {

                for(int j=0; j<Assets.Count; j++)
                {
                    double[] Path = Assets[j].GeneratePath(NumSamples, Tau);
                    Aux2[j] = AuxFunction(Path);
                    
                }
                
                double Payoff = Aux2.Min();

                Interlocked.Exchange(ref Aux, Payoff + Aux);

            });
            return Math.Exp(-RiskFreeRate * OptionExercise) * Aux / NPaths;

        }

    }
}
