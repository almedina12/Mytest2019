using HestonModel.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HestonModel
{
    public class HestonCalibrationResult : IHestonCalibrationResult
    {
        public IHestonModelParameters Parameters { get; }

        public CalibrationOutcome MinimizerStatus { get; }

        public double PricingError { get; }

        public HestonCalibrationResult(double PricingError, IHestonModelParameters Parameters, CalibrationOutcome MinimizerStatus)
        {
            this.PricingError = PricingError;
            this.Parameters = Parameters;
            this.MinimizerStatus = MinimizerStatus;
        }
    }
}
