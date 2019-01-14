using HestonModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HestonModel
{
    public class CalibrationSettings : ICalibrationSettings
    {
        public double Accuracy { get; }

        public int MaximumNumberOfIterations { get; }

        public CalibrationSettings(double Accuracy, int MaximumNumberOfIterations)
        {
            this.Accuracy = Accuracy;
            this.MaximumNumberOfIterations = MaximumNumberOfIterations;
        }

    }
}