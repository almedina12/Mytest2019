using HestonModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HestonModel.TheClasses
{
    public class AsianOption : IAsianOption
    {
        public IEnumerable<double> MonitoringTimes { get; }

        public PayoffType Type { get; }

        public double StrikePrice { get; }

        public double Maturity { get; }

        public AsianOption(double StrikePrice, double Maturity, IEnumerable<double> MonitoringTimes, PayoffType Type)
        {
            this.StrikePrice = StrikePrice;
            this.Maturity = Maturity;
            this.MonitoringTimes = MonitoringTimes;
            this.Type = Type;
        }
    }
}
