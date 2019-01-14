using HestonModel.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HestonModel.TheClasses
{
    public class HestonEnumerable2 : IEnumerable<IOptionMarketData<IEuropeanOption>>
    {
        public List<IOptionMarketData<IEuropeanOption>> Data;

        public IOptionMarketData<IEuropeanOption> this[int index]
        {
            get { return Data[index]; }
            set { Data.Insert(index, value); }
        }

        public HestonEnumerable2()
        {
            List<IOptionMarketData<IEuropeanOption>> Data = new List<IOptionMarketData<IEuropeanOption>>();
            this.Data = Data;
        }

        public IEnumerator<IOptionMarketData<IEuropeanOption>> GetEnumerator()
        {
            return Data.GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<IOptionMarketData<IEuropeanOption>> IEnumerable<IOptionMarketData<IEuropeanOption>>.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

}
