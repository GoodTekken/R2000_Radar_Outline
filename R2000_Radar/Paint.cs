using R2000_Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace R2000_Radar
{
    public class Paint:Form
    {
        
        string datastream;
        bool showstyle;
        float addsd;

        public string Datastream { get => datastream; set => datastream = value; }
        public bool Showstyle { get => showstyle; set => showstyle = value; }

        public Paint()
        {
            this.Width = 1300;
            this.Height = 800;
            this.Text = "Paint";

            addsd = 0.1f;

            Button bt_Show = new System.Windows.Forms.Button();
            // 
            // bt_Show
            // 
            bt_Show.Location = new System.Drawing.Point(12, 12);
            bt_Show.Name = "bt_Show";
            bt_Show.Size = new System.Drawing.Size(75, 23);
            bt_Show.TabIndex = 0;
            bt_Show.Text = "显示";
            bt_Show.UseVisualStyleBackColor = true;
            bt_Show.Click += new System.EventHandler(this.bt_Show_Click);
            this.Controls.Add(bt_Show);

            this.MouseWheel += new MouseEventHandler(Paint_MouseWheel);

        }

        public void Paint_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
                addsd -= 0.01f;
            else
                addsd += 0.01f;
            if (addsd >= 0.3)
                addsd = 0.3f;
            if (addsd <= 0.02)
                addsd = 0.02f;

            Console.WriteLine(addsd);
        }


        private void bt_Show_Click(object sender, EventArgs e)
        {
            //Initialize all called on classes, any new classes added need to be added below.
            //From R2000 Library:
            Thread receiverThread = new Thread(R2000_Command);
            receiverThread.IsBackground = true;
            receiverThread.Start();
        }

        public void R2000_Command()
        {
            Configuration configuration = new Configuration();
            Command command = new Command();
            Connect connect = new Connect();
            Data data = new Data();
            //From R2000 Measurement:
            Calculations calculations = new Calculations();

            while (true)
            {
                //******** Start writing your code below this line ************
                configuration.config();
                connect.connecttcp();
                connect.gettcpsocket();

                //command.setparameters();
                command.getparameters();
                //command.setscanoutputconfig();
                command.getscanoutputconfig();

                command.watchdog();


                command.startstream();
                data.initialize(); //This gets our size of the scan Var.byteamount

                data.background();

                command.stopstream();
                command.handlerelease();

                Console.WriteLine("Done!");
                Paint_Function(Laser_Style.Line);
                Thread.Sleep(5000); //1500
            }
        }
        /// <summary>
        /// 显示画面
        /// </summary>
        /// <param name="style">false:显示:Line,false:显示:点</param>
        public void Paint_Function(Laser_Style laserstyle)
        {
            int dataSize;
            try
            {
                //if (Var.measurmentdata.Length < 6300)
                //{
                //    return;
                //}
                dataSize = Var.measurmentdata.Length;  //不使用过滤模式：6300，使用Average过滤模式:1575


            }
            catch (Exception)
            {
                //throw;
                return;
            }

            int[] data = new int[dataSize];

            float length;
            float theta;
            float[] lengthX = new float[dataSize];
            float[] lengthY = new float[dataSize];
            //定义窗体大小
            int halfWidth = this.Width / 2;
            int halfHeight = this.Height / 2;
            Console.WriteLine("Paint窗口的X坐标：" + halfWidth);
            Console.WriteLine("Paint窗口的X坐标："  + halfHeight);
            Console.WriteLine("接收到的数据长度 ：" + data.Length);

            Graphics g = CreateGraphics();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (int)(Var.background[i] * addsd);
                //data[i] = (int)(Var.measurmentdata[i] * addsd);
            }


            for (int i = 0; i < dataSize; i++)      //800  813
            {
                length = data[i];
                float aux1 = dataSize / 90;
                theta = (float)(i * 4 / aux1 +  90);                     //(float)((i / 3) - 45);  (float)(i * 4 / 70 +  90);=> 6300 
                lengthX[i] = length * (float)(Math.Cos(Math.PI * theta / 180));
                lengthY[i] = length * (float)(Math.Sin(Math.PI * theta / 180));
            }

            g.Clear(this.BackColor);   //清除画面
            Pen laser = new Pen(Color.Red, 1);

            switch (laserstyle)
            {
                case Laser_Style.Point:
                    Point[] point = new Point[dataSize];
                    for (int i = 0; i < dataSize; i++)      //800  813
                    {
                        point[i].X = (int)(halfWidth + lengthX[i]);
                        point[i].Y = (int)(halfHeight - lengthY[i]);
                    }
                    g.DrawLines(laser, point);
                    break;
                case Laser_Style.Line:
                    for (int i = 0; i < dataSize; i++)      //800  813   //下面是划线，有延时
                    {
                        g.DrawLine(laser, halfWidth, halfHeight, halfWidth + lengthX[i], halfHeight- lengthY[i]);
                    }
                    break;
                default:
                    break;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Paint";
            this.Text = "Paint";
            this.ResumeLayout(false);

        }
    }
}
