using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TipCalculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void billBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTip();
        }

        private  bool UpdateTip()
        {
            if(!double.TryParse(billBox.Text, out double unused) || !double.TryParse
                (tipPercentage.Text, out double unused2))
            {
                return false;
            }
            else
            {
                double bill = Convert.ToDouble(billBox.Text);
                double percentage = Convert.ToDouble(percentageAmt.Text);
                double tip = bill * percentage / 100;
                double totalAmount = bill + tip;
                tipAmt.Text = tip.ToString();
                finishedBill.Text = (((Convert.ToDouble(billBox.Text) * tip).ToString()));
                return true;
            }
        }

        private void percentageAmt_TextChanged(object sender, EventArgs e)
        {
            UpdateTip();
        }
    }


}
