using HestonModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HestonModel
{
    public class HestonModelParameters : IHestonModelParameters
    {
        public double InitialStockPrice { get; }

        public double RiskFreeRate { get; }

        public IVarianceProcessParameters VarianceParameters { get; }

        public HestonModelParameters(double InitialStockPrice, double RiskFreeRate, IVarianceProcessParameters VarianceParameters)
        {
            this.InitialStockPrice = InitialStockPrice;
            this.RiskFreeRate = RiskFreeRate;
            this.VarianceParameters = VarianceParameters;
        }

    }
}
