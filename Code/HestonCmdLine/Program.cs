using System;
using FinalProject;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HestonModel;

namespace HestonCmdLine
{
    /// <summary>
    /// This is the main class of the Console app where all the other functions are called. I used this for debugging.
    /// The purpose of this is to expose all of the functionality implemented in all the projects.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            //Methods in the result class:

            //---------------------------------
            //Result.HestonFormulaResult();
            //Result.HestonMonteCarloResult();
            //Result.AsianOptionsResult();
            //Result.LookbackOptionsResult();
            //Result.CliquetOptionResult();
            //Result.EverestOptionsResult();
            //---------------------------------

            //CheckingCalibration.CheckCalibration();

            //Methods for the correct set up.

            //---------------------------------
            //CorrectSetUp.FormulaSetUp();
            //CorrectSetUp.MonteCarloSetUp();
            //CorrectSetUp.LookbackOptionSetUp();
            //CorrectSetUp.AsianOptionsSetUp();
            //CorrectSetUp.CalibrationSetUp();
            //---------------------------------

        }
    }
}
