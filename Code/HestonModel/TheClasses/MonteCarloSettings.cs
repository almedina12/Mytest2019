using HestonModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HestonModel.TheClasses
{
    public class MonteCarloSettings : IMonteCarloSettings
    {
        public int NumberOfTrials { get;  }

        public int NumberOfTimeSteps { get; }

        public MonteCarloSettings(int NumberOfTrials, int NumberOfTimeSteps)
        {
            this.NumberOfTimeSteps = NumberOfTimeSteps;
            this.NumberOfTrials = NumberOfTrials;
        }
    }
}
