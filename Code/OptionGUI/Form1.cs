using FinalProject;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OptionGUI
{
    /// <summary>
    /// This class contains all functionality related with the GUI. Here most of the classes of FinalProjet are called.
    /// </summary>
    public partial class OptionCalculator : Form
    {
        public OptionCalculator()
        {
            InitializeComponent();

        }

        double RiskFreeRate;
        double StrikePrice;
        double OptionExcercise;
        double Sigma;
        double InitialStockPrice;
        double Kappa;
        double Theta;
        double Rho;
        double Nu;
        int TimeSteps;
        int Paths;

        /// <summary>
        /// If the button is clicked the price is calculated based on the information provided in the text boxes and the combo box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {



            RiskFreeRate = double.Parse(textBox2.Text);
            StrikePrice = double.Parse(textBox3.Text);
            OptionExcercise = double.Parse(textBox4.Text);
            Sigma = double.Parse(textBox9.Text);
            InitialStockPrice = double.Parse(textBox6.Text);




            double Price = 0;

            if (comboBox2.Text == "Heston Formula") {

                Kappa = double.Parse(textBox11.Text);
                Theta = double.Parse(textBox10.Text);
                Rho = double.Parse(textBox8.Text);
                Nu = double.Parse(textBox7.Text);


                HestonFormula Formula = new HestonFormula(RiskFreeRate, InitialStockPrice, Kappa, Theta, Sigma, Rho, Nu);



                if (comboBox1.Text == "Call")
                {
                    Price = Formula.CalculateCallPrice(StrikePrice, OptionExcercise);
                }
                else if (comboBox1.Text == "Put")
                {
                    Price = Formula.CalculatePutPrice(StrikePrice, OptionExcercise);
                }
                


            }else if (comboBox2.Text == "Heston Monte Carlo")
            {
                Kappa = double.Parse(textBox11.Text);
                Theta = double.Parse(textBox10.Text);
                Rho = double.Parse(textBox8.Text);
                Nu = double.Parse(textBox7.Text);
                TimeSteps = int.Parse(textBox16.Text);
                Paths = int.Parse(textBox15.Text);



                HestonMonteCarlo MonteCarlo = new HestonMonteCarlo(RiskFreeRate, StrikePrice, InitialStockPrice, 
                    Kappa, Theta, Sigma, Rho, Nu, TimeSteps);

                if (comboBox1.Text == "Call")
                {
                    Price = MonteCarlo.CalculatePrice(OptionExcercise, Paths);
                }
                else if (comboBox1.Text == "Put")
                {
                    Price = MonteCarlo.CalculatePutPrice(OptionExcercise, Paths);
                }
                

                
                
            }
            else if (comboBox2.Text == "Lookback")
            {
                Kappa = double.Parse(textBox11.Text);
                Theta = double.Parse(textBox10.Text);
                Rho = double.Parse(textBox8.Text);
                Nu = double.Parse(textBox7.Text);
                TimeSteps = int.Parse(textBox16.Text);
                Paths = int.Parse(textBox15.Text);

                LookbackOptions MonteCarlo = new LookbackOptions(RiskFreeRate, InitialStockPrice,
                Kappa, Theta, Sigma, Rho, Nu, TimeSteps);


                Price = MonteCarlo.CalculatePrice(OptionExcercise, Paths);
                

            }
            else if (comboBox2.Text == "Cliquet")
            {

                double Aux1 = double.Parse(textBox12.Text);
                double Aux2 = double.Parse(textBox17.Text);
                double Aux3 = double.Parse(textBox18.Text);

                double[] FixingDates = { Aux1, Aux2, Aux3};
                
                double PayoutRate = double.Parse(textBox13.Text);

                int Lambda = -1;

                if (comboBox1.Text == "Call")
                {
                    Lambda = 1;
                }


                CliquetOptions Option = new CliquetOptions(RiskFreeRate, FixingDates, Lambda, Sigma, StrikePrice, InitialStockPrice, PayoutRate);
                Price = Option.CalculatePrice();

            }




            textBox1.Text = Price.ToString();


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox2.Text == "Plot Convergence (Monte Carlo and Formula)")
            {
                button1.Enabled = false;
                comboBox1.Enabled = true;
                textBox11.Enabled = true;
                textBox10.Enabled = true;
                textBox8.Enabled = true;
                textBox7.Enabled = true;
                textBox4.Enabled = true;
                textBox16.Enabled = true;
                textBox15.Enabled = true;
                textBox12.Enabled = false;
                textBox17.Enabled = false;
                textBox18.Enabled = false;
                textBox13.Enabled = false;
                PlotConvergence.Visible = true;
            }
            else
            {
                PlotConvergence.Visible = false;
                button1.Enabled = true;
            }

            if (comboBox2.Text == "Heston Formula")
            {

                comboBox1.Enabled = true;
                textBox11.Enabled = true;
                textBox10.Enabled = true;
                textBox8.Enabled = true;
                textBox7.Enabled = true;
                textBox4.Enabled = true;
                textBox16.Enabled = false;
                textBox15.Enabled = false;
                textBox12.Enabled = false;
                textBox17.Enabled = false;
                textBox18.Enabled = false;
                textBox13.Enabled = false;
            }else if(comboBox2.Text == "Heston Monte Carlo" )
            {
                comboBox1.Enabled = true;
                textBox11.Enabled = true;
                textBox10.Enabled = true;
                textBox8.Enabled = true;
                textBox7.Enabled = true;
                textBox4.Enabled = true;
                textBox16.Enabled = true;
                textBox15.Enabled = true;
                textBox12.Enabled = false;
                textBox17.Enabled = false;
                textBox18.Enabled = false;
                textBox13.Enabled = false;

            }else if (comboBox2.Text == "Lookback")
            {
                textBox11.Enabled = true;
                textBox10.Enabled = true;
                textBox8.Enabled = true;
                textBox7.Enabled = true;
                textBox4.Enabled = true;
                textBox16.Enabled = true;
                textBox15.Enabled = true;
                textBox12.Enabled = false;
                textBox17.Enabled = false;
                textBox18.Enabled = false;
                textBox13.Enabled = false;
                comboBox1.Enabled = false;
            }
            else if (comboBox2.Text == "Cliquet")
            {
                comboBox1.Enabled = true;
                textBox11.Enabled = false;
                textBox10.Enabled = false;
                textBox8.Enabled = false;
                textBox7.Enabled = false;
                textBox16.Enabled = false;
                textBox15.Enabled = false;
                textBox4.Enabled = false;
                textBox12.Enabled = true;
                textBox17.Enabled = true;
                textBox18.Enabled = true;
                textBox13.Enabled = true;
            }
               
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox17_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox18_TextChanged(object sender, EventArgs e)
        {

        }

        private void PlotConvergence_Click(object sender, EventArgs e)
        {
            Form2 PlotConvergenceForm = new Form2();
            PlotConvergenceForm.Show();
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
