using HestonModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HestonModel
{
    public class VarianceProcessParameters : IVarianceProcessParameters
    {
        public double Kappa { get; }

        public double Theta { get; }

        public double Sigma { get; }

        public double V0 { get; }

        public double Rho { get; }

        public VarianceProcessParameters(double Kappa, double Theta, double Sigma, double V0, double Rho)
        {
            this.Kappa = Kappa;
            this.Theta = Theta;
            this.Sigma = Sigma;
            this.V0 = V0;
            this.Rho = Rho;
        }

    }
}
