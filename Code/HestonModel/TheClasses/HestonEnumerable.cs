using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HestonModel.TheClasses
{
    public class HestonEnumerable : IEnumerable<double>
    {
        public List<double> Dates;

        public double this[int index]
        {
            get { return Dates[index]; }
            set { Dates.Insert(index, value); }
        }

        public HestonEnumerable()
        {
            List<double> Dates_ = new List<double>();
            this.Dates = Dates_;
        }

        public HestonEnumerable(double[] Dates)
        {
            List<double> Dates_ = new List<double>(Dates);
            this.Dates = Dates_;
        }

        public IEnumerator<double> GetEnumerator()
        {
            return Dates.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }
}
