using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormCalculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            label1.Text = "-";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "+";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label1.Text = "*";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = "/";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Please input the two operands!", "Note", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                double x, y;
                x = Convert.ToDouble(textBox1.Text);
                y = (textBox2.Text == "") ? 0 : Convert.ToDouble(textBox2.Text);

                switch (Convert.ToChar(label1.Text))
                {
                    case '+':      
                        label3.Text = (x + y).ToString();
                        break;
                    case '-':
                        label3.Text = (x - y).ToString();
                        break;
                    case '*':
                        label3.Text = (x * y).ToString();
                        break;
                    case '/':
                        if (y == 0)
                        {
                            label3.Text = "Infinite";
                        }
                        else
                        {
                            label3.Text = (x / y).ToString();
                        }
                        break;
                    default:
                        MessageBox.Show("Please select the operator!", "Note", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            label3.Text = "";
        }
    }
}
