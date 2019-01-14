
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HestonModel;
using HestonModel.Interfaces;

namespace HestonModel
{
    public class EuropeanOption : IEuropeanOption
    {
        public PayoffType Type { get; }
        public double StrikePrice { get; }
        public double Maturity { get; }

        public EuropeanOption(double StrikePrice, double Maturity, PayoffType Type)
        {
            this.StrikePrice = StrikePrice;
            this.Maturity = Maturity;
            this.Type = Type;
        }

    }
}