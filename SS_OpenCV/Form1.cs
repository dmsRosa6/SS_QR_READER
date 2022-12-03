using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SS_OpenCV
{
    public partial class Form1 : Form
    {
        public float[,] matrix = new float[3, 3];
        public float weight, offset;

        public Form1()
        {
            InitializeComponent();
        }

        //Cancel Button
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        //Ok click
        private void button1_Click(object sender, EventArgs e)
        {
            if(!float.TryParse(textBox1.Text,out matrix[0, 0]))
            {
                MessageBox.Show("Error");
                return;
            }

            if (!float.TryParse(textBox2.Text, out matrix[0, 1]))
            {
                MessageBox.Show("Error");
                return;
            }

            if (!float.TryParse(textBox3.Text, out matrix[0, 2]))
            {
                MessageBox.Show("Error");
                return;
            }

            if (!float.TryParse(textBox4.Text, out matrix[1, 0]))
            {
                MessageBox.Show("Error");
                return;
            }

            if (!float.TryParse(textBox5.Text, out matrix[1, 1]))
            {
                MessageBox.Show("Error");
                return;
            }

            if (!float.TryParse(textBox6.Text, out matrix[1, 2]))
            {
                MessageBox.Show("Error");
                return;
            }

            if (!float.TryParse(textBox7.Text, out matrix[2, 0]))
            {
                MessageBox.Show("Error");
                return;
            }

            if (!float.TryParse(textBox8.Text, out matrix[2, 1]))
            {
                MessageBox.Show("Error");
                return;
            }

            if (!float.TryParse(textBox9.Text, out matrix[2, 2]))
            {
                MessageBox.Show("Error");
                return;
            }

            if (!float.TryParse(textBox10.Text, out weight))
            {
                MessageBox.Show("Error");
                return;
            }

            if (!float.TryParse(textBox11.Text, out offset))
            {
                MessageBox.Show("Error");
                return;
            }
                DialogResult = DialogResult.OK;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    textBox1.Text = "-1";
                    textBox2.Text = "-1";
                    textBox3.Text = "-1";
                    textBox4.Text = "-1";
                    textBox5.Text = "9";
                    textBox6.Text = "-1";
                    textBox7.Text = "-1";
                    textBox8.Text = "-1";
                    textBox9.Text = "-1";
                    textBox10.Text = "1";
                    textBox11.Text = "0";
                    break;

                case 1:
                    textBox1.Text = "1";
                    textBox2.Text = "2";
                    textBox3.Text = "1";
                    textBox4.Text = "2";
                    textBox5.Text = "4";
                    textBox6.Text = "2";
                    textBox7.Text = "1";
                    textBox8.Text = "2";
                    textBox9.Text = "1";
                    textBox10.Text = "16";
                    textBox11.Text = "0";
                    break;

                case 2:
                    textBox1.Text = "1";
                    textBox2.Text = "-2";
                    textBox3.Text = "1";
                    textBox4.Text = "-2";
                    textBox5.Text = "4";
                    textBox6.Text = "-2";
                    textBox7.Text = "1";
                    textBox8.Text = "-2";
                    textBox9.Text = "1";
                    textBox10.Text = "1";
                    textBox11.Text = "0";
                    break;

                case 3:
                    textBox1.Text = "0";
                    textBox2.Text = "0";
                    textBox3.Text = "0";
                    textBox4.Text = "-1";
                    textBox5.Text = "2";
                    textBox6.Text = "-1";
                    textBox7.Text = "0";
                    textBox8.Text = "0";
                    textBox9.Text = "0";
                    textBox10.Text = "1";
                    textBox11.Text = "128";
                    break;
            }

            DialogResult = DialogResult.OK;
        }
        
    }
}
