using System;
using MathNet.Numerics.Distributions;
using System.Threading.Tasks;
using System.Threading;

namespace FinalProject
{
    /// <summary>
    /// Class for pricing European put and call options using Monte Carlo with the Heston model.
    /// <see href="final-project-almedina12/HestonModel.pdf">Reference.</see>
    /// </summary>
    /// <remarks>
    /// This is also a parent class for other types of options in the same framework, 
    /// therefore the global variables are protected.
    /// </remarks>
    public class HestonMonteCarlo
    {
        protected double RiskFreeRate { get; set; }
        protected double StrikePrice { get; set; }
        protected double InitialStockPrice { get; set; }
        protected double Kappa { get; set; }
        protected double Theta { get; set; }
        protected double Sigma { get; set; }
        protected double Rho { get; set; }
        protected double Nu { get; set; }
        protected int TimeSteps { get; set; }

        protected double Alpha;
        protected double Beta;
        protected double Gamma;


        /// <summary>
        /// Constructor for the class. Global variables <paramref name="Alpha"/>, <paramref name="Beta"/> 
        /// and <paramref name="Gamma"/> are calculated.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when one of the 
        /// arguments provided to a method is not valid.</exception>
        public HestonMonteCarlo(
            double RiskFreeRate, 
            double StrikePrice, 
            double InitialStockPrice,
            double Kappa, 
            double Theta, 
            double Sigma, 
            double Rho,
            double Vu,
            int TimeSteps =365)

        {

            this.RiskFreeRate = RiskFreeRate;
            this.StrikePrice = StrikePrice;
            this.InitialStockPrice = InitialStockPrice;
            this.Kappa = Kappa;
            this.Theta = Theta;

            if (Sigma < 0)
                throw new ArgumentException("--- Error: Sigma must not be negative. ---");
            this.Sigma = Sigma;

            if (Math.Abs(Rho) > 1)
                throw new ArgumentException("--- Error: Rho must be less than 1. ---");
            this.Rho = Rho;

            this.Nu = Vu;

            if (TimeSteps <= 0)
                throw new ArgumentException("--- Error: The number of time steps must be a positive integer. ---");
            this.TimeSteps = TimeSteps;

            if (2 * Kappa * Theta <= Math.Pow(Sigma, 2))
                throw new ArgumentException("--- Error: Feller Condition not met. ---");

            Alpha = (4 * Kappa * Theta - Math.Pow(Sigma, 2)) / 8;
            Beta = -Kappa / 2;
            Gamma = Sigma / 2;

        }

        /// <summary>
        /// Generates samples from a Normal distribution with mean 0 and variance 1.
        /// </summary>
        /// <returns>
        /// Array of doubles.
        /// </returns>
        /// <param name="NumSamples">Integer. Number of Samples.</param>
        protected double[] GenerateSample(int NumSamples)
        {
            double[] Z = new double[NumSamples];
            Normal.Samples(Z, 0, 1);
            return Z;
        }

        /// <summary>
        /// Payoff of the option. It is virtual to allow for it to be overridden.
        /// </summary>
        /// <returns>
        /// Double precision number.
        /// </returns>
        /// <param name="Path"> Array of doubles that represent the path of the underlaying asset.</param>
        /// <remarks>
        /// In this case the payoff is that of a European Call option.
        /// </remarks>
        protected virtual double Payoff(double[] Path)
        {
            return Math.Max(0, Path[Path.Length - 1] - StrikePrice);
        }

        /// <summary>
        /// Generates one path for the option, based on the Monte Carlo Formulas from the Heston Model.
        /// </summary>
        /// <returns>
        /// The path, which is an array of doubles, represents the price as function of time.
        /// </returns>
        /// <param name="NumSamples"> Number of elements of the path.</param>
        /// <param name="Tau"> Represents the marginal increment of time.</param>
        /// <exception cref="DivideByZeroException">
        /// Thrown when:
        /// <code>
        /// Beta * Tau == 1
        /// </code>
        /// </exception>
        public double[] GeneratePath(int NumSamples, double Tau)
        {
            double[] SamplePaths = new double[NumSamples];
            double[] X1 = GenerateSample(NumSamples);
            double[] X2 = GenerateSample(NumSamples);
            double DeltaZ1;
            double DeltaZ2;
            double Aux;
            double Y = Math.Sqrt(Nu);

            SamplePaths[0] = InitialStockPrice;

            for (int k = 1; k < NumSamples; k++)
            {
                DeltaZ1 = Math.Sqrt(Tau) * X1[k-1];
                DeltaZ2 = Math.Sqrt(Tau) * (Rho * X1[k-1] + Math.Sqrt(1 - Math.Pow(Rho, 2)) * X2[k-1]);
                SamplePaths[k] = SamplePaths[k-1] + RiskFreeRate * SamplePaths[k-1] * Tau + Y * SamplePaths[k-1] * DeltaZ1;

                if (Beta * Tau == 1) throw new DivideByZeroException();

                Aux = (Y + Gamma * DeltaZ2) / (2 * (1 - Beta * Tau));
                Y = Aux + Math.Sqrt(Aux * Aux + Alpha * Tau / (1 - Beta * Tau));
            }

            return SamplePaths;

        }

        /// <summary>
        /// Calculate the approximate Call price of an option using Monte Carlo and the Heston Model.
        /// </summary>
        /// <returns>
        /// The approximate Call price of the option.
        /// </returns>
        /// <param name="OptionExcercise">The option exercise date of the option.</param>
        /// <param name="NPaths">Number of paths used in the Monte Carlo algorithm.</param>
        /// <remarks>
        /// Given that in Normal.Samples() every sample is independent, we can use parallel threads.
        /// </remarks>
        public double CalculatePrice(double OptionExercise, int NPaths = 10000)
        {
            if (OptionExercise <= 0)
                throw new ArgumentException("--- Error: Option exercise date must be greater than 0. ---");

            if (NPaths <= 0)
                throw new ArgumentException("--- Error: The number of paths must be a positive integer. ---");

            int NumSamples = (int)Math.Ceiling(OptionExercise * TimeSteps);
            double Tau = OptionExercise / NumSamples;

            double Aux = 0;
            double[] Path;


            Parallel.For(0, NPaths, i => {
                Path = GeneratePath(NumSamples,Tau);
                Interlocked.Exchange(ref Aux, Payoff(Path) + Aux);
            });
            return Math.Exp(-RiskFreeRate * OptionExercise) * Aux / NPaths;
        }


        /// <summary>
        /// Formula for the Put option based on the Put-Call option parity.
        /// </summary>
        /// <returns>
        /// The approximate Put price of the option.
        /// </returns>
        /// <param name="OptionExcercise">The option exercise date of the option.</param>
        /// <param name="NPaths">Number of paths used in the Monte Carlo algorithm.</param>
        public virtual double CalculatePutPrice(double OptionExercise,  int NPaths = 10000)
        {
            double CallPrice = CalculatePrice(OptionExercise, NPaths);
            return CallPrice + StrikePrice * Math.Exp(-RiskFreeRate * OptionExercise) - InitialStockPrice;
        }




    }
}
