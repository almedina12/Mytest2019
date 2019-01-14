
using HestonModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HestonModel
{
    public class OptionMarketData : IOptionMarketData<IEuropeanOption>
    {


        public double Price { get; set; }

        public IEuropeanOption Option { get; set; }

        public OptionMarketData(EuropeanOption Option, double Price)
        {
            this.Option = Option;
            this.Price = Price;
        }

    }
}
