using R2000_Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace R2000_Radar
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Paint myPaint = new Paint();
            myPaint.UserEvent += myPaint.SendCountToTextBox;
            //myPaint.Paint_Function(Laser_Style.Line);
            //this.Hide();
            myPaint.ShowDialog();
        }



        /// <summary>
        /// 分辨率0.0571°，共有6300的数据项，从R2000的-180开始逆时针扫描；
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Get the data of R2000：");
            for (int i = 0; i < Var.background.Length; i++)
            {
                Console.WriteLine((Var.background[i]).ToString() + " ");
                richTextBox1.Text += ((Var.background[i]).ToString() + " ");
            }
        }
    }
}
