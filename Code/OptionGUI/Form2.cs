using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using FinalProject;
using FinalProjectTests;

namespace OptionGUI
{
    public partial class Form2 : Form
    {


        public Form2()
        {
            InitializeComponent();

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// This method produces the charts that we see in the GUI. It first calls the ConvergenceHestonMonteCarlo
        /// class that calculates the error and then produces two series with all its features. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {


            label6.Show();

            ConvergenceHestonMonteCarlo Function = new ConvergenceHestonMonteCarlo();

            int[] X = { 1, 10, 100, 1000, 10000, 30000};
            double[] Y1 = Function.DataPointsPath(X);
            double[] Y2 = Function.DataPointsTimeSteps(X);
            chart1.Series.Clear();
            chart2.Series.Clear();

            var Series1 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Series1",
                Color = System.Drawing.Color.Green,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Spline
            };

            var Series2 = new System.Windows.Forms.DataVisualization.Charting.Series
            {
                Name = "Series2",
                Color = System.Drawing.Color.Blue,
                IsVisibleInLegend = false,
                IsXValueIndexed = true,
                ChartType = SeriesChartType.Spline
            };

            chart1.Series.Add(Series1);
            chart2.Series.Add(Series2);

            for (int i=1;i<X.Length; i++)
            {
                chart1.Series["Series1"].Points.AddXY(X[i], Y1[i]);
                chart2.Series["Series2"].Points.AddXY(X[i], Y2[i]);
            }
            label6.Hide();
            chart1.ChartAreas[0].AxisX.IsMarginVisible = false;
            chart2.ChartAreas[0].AxisX.IsMarginVisible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            this.Hide();
            
        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }

        private void eventLog2_EntryWritten(object sender, System.Diagnostics.EntryWrittenEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
    }
}
