using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint
{
    public partial class Form1 : Form
    {
        Graphics g;
        Pen laser;
        Pen outLine;
        Pen bluePen;
        Pen redPen;
        Pen blackPen;
        int halfWidth;
        int halfHeight;
        Point[] point = new Point[6300];
        Point[] Drawpoint = new Point[6300];
        Random rnd = new Random();

        float addsd;
        int success_count;

        public Form1()
        {
            InitializeComponent();
            g = CreateGraphics();
            laser = new Pen(Color.Red, 1);
            outLine = new Pen(Color.Black, 3);

            redPen = new Pen(Color.Red, 1);
            bluePen = new Pen(Color.Blue, 1);
            blackPen = new Pen(Color.Black, 1);

            halfWidth = this.Width / 2;
            halfHeight = this.Height / 2;
            for (int i = 0; i < 5; i++)
            {
                point[i].X = rnd.Next(50 * i, 50 * (i + 1));
                point[i].Y = rnd.Next(50 * i, 50 * (i + 1));
            }

            addsd = 0.03f;
            this.MouseWheel += new MouseEventHandler(Paint_MouseWheel);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                point[i].X = rnd.Next(50 *i, 50 * (i+1));
                point[i].Y = rnd.Next(50 *i, 50 * (i+1));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)      //800  813
            {
                Drawpoint[i].X = (int)(halfWidth + point[i].X);
                Drawpoint[i].Y = (int)(halfHeight - point[i].Y);
            }
            g.DrawLines(bluePen, Drawpoint);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            g.Clear(this.BackColor);   //清除画面
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)      //
            {
                g.DrawLine(blackPen, halfWidth, halfHeight, halfWidth + point[i].X, halfHeight - point[i].Y);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int R = 5;
            for (int i = 0; i < 5; i++)      //
            {
                g.DrawEllipse(redPen, (halfWidth + point[i].X - R), (halfHeight - point[i].Y - R), R*2,R*2 );
            }
        }


        private void button6_Click(object sender, EventArgs e)
        {
            int point_count = 0;

            for (int i = 0; i < point.Length; i++)   //  边缘检测
            {
                if ((point[i].X > 0) && point[i].Y > 0)
                {
                    point_count++;
                }
            }

            Point[] point_select = new Point[point_count];

            int j = 0;
            for (int i = 0; i < point.Length; i++)   //  边缘检测
            {
                if ((point[i].X > 0) && point[i].Y > 0)
                {
                    point_select[j].X = point[i].X;
                    point_select[j].Y = point[i].Y;
                    j++;
                }
            }

            line_struct a = lineCalculate(point_select);
            richTextBox1.Text += string.Format("RCA:{0}\n", a.RCA);
            richTextBox1.Text += string.Format("RCB:{0}\n", a.RCB);
            richTextBox1.Text += string.Format("Angle:{0}\n", a.angle);
            //richTextBox1.Text += string.Format("getDistanceFormPointToLine:{0}\n", (int)getDistanceFormPointToLine(1,1,1,1,0));

            Point startPoint = new Point();
            Point endPoint = new Point();
            startPoint = getFootOfPerpendicular((int)point[0].X, (int)point[0].Y, a.RCB, -1, a.RCA);
            endPoint = getFootOfPerpendicular((int)point[point_count - 1].X, (int)point[point_count - 1].Y, a.RCB, -1, a.RCA);

            g.DrawLine(laser, halfWidth + startPoint.X, halfHeight - startPoint.Y, halfWidth + endPoint.X, halfHeight - endPoint.Y);
            g.DrawLine(bluePen, halfWidth + point[0].X, halfHeight - (point[0].X * a.RCB + a.RCA), halfWidth + point[point_count - 1].X, halfHeight - (point[point_count - 1].X * a.RCB + a.RCA));

            for (int i = 0; i < point_count; i++)
            {
                double distance = getDistanceFormPointToLine((int)point[i].X, (int)point[i].Y, a.RCB, -1, a.RCA);
                richTextBox1.Text += string.Format("Point:{0}的直线距离是：{1}\n", i, distance);
            }
        }

        public line_struct lineCalculate(Point[] point_select)
        {
            ArrayList pointX = new ArrayList();
            ArrayList pointY = new ArrayList();
            float[] ArrPoint_X = new float[400];
            float[] ArrPoint_Y = new float[400];

            int ArrPoint_Count = 0;
            pointX.Clear();
            pointY.Clear();
            try
            {
                for (int i = 0; i < point_select.Length; i++)   //  边缘检测
                {
                        pointX.Add(point_select[i].X);
                        pointY.Add(point_select[i].Y);
                        ArrPoint_X[ArrPoint_Count] = point_select[i].X;
                        ArrPoint_Y[ArrPoint_Count] = point_select[i].Y;
                        ArrPoint_Count++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            float averagex = 0, averagey = 0;
            foreach (int i in pointX)
            {
                averagex += i;
            }
            foreach (int j in pointY)
            {
                averagey += j;
            }
            averagex /= pointX.Count;        //  取X坐标的平均数
            averagey /= pointY.Count;        //  取Y坐标的平均数

            //经验回归系数的分子与分母
            float numerator = 0;
            float denominator = 0;
            for (int i = 0; i < pointX.Count; i++)
            {
                numerator += (ArrPoint_X[i] - averagex) * (ArrPoint_Y[i] - averagey);
                denominator += (ArrPoint_X[i] - averagex) * (ArrPoint_X[i] - averagex);
            }
            //回归系数b（Regression Coefficient）   y = bx + a ; ==> bx-y+a =0
            float RCB = numerator / denominator;
            //回归系数a
            float RCA = averagey - RCB * averagex;
            double angle = 180 * Math.Atan(RCB) / Math.PI;

            line_struct param = new line_struct();
            param.RCB = RCB;
            param.RCA = RCA;
            param.angle = angle;
            return param;
        }

        /// <summary>
        /// 求点（x1,y1）与直线Ax+By+C = 0的交点坐标
        /// </summary>
        /// <returns></returns>
        private static Point getFootOfPerpendicular(int x1, int y1, double A, double B, double C)
        {
            if (A * A + B * B < 1e-13)
                return new Point(0,0);

            if (Math.Abs(A * x1 + B * y1 + C) < 1e-13)
            {
                return new Point(x1, y1);
            }
            else
            {
                int newX = (int)((B * B * x1 - A * B * y1 - A * C) / (A * A + B * B));
                int newY = (int)((-A * B * x1 + A * A * y1 - B * C) / (A * A + B * B));
                return new Point(newX, newY);
            }
        }

        /// <summary>
        /// 求点（x1,y1）与直线Ax+By+C = 0的距离
        /// </summary>
        /// <returns></returns>
        private static double getDistanceFormPointToLine(int x1,int y1, double A, double B, double C)
        {
            double distance = 0;
            double numerator = A * x1 + B * y1 + C;
            double denominator = A * A + B * B;
            distance = (Math.Abs(numerator) / Math.Sqrt(denominator));
            return distance;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                // 创建一个 StreamReader 的实例来读取文件 
                // using 语句也能关闭 StreamReader
                //using (StreamReader sr = new StreamReader("LS2000Data.txt"))
                using (StreamReader sr = new StreamReader("data20200619.txt"))
                {
                    string line;
                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        string myByteArray = line;
                        string[] datas = myByteArray.Split(' ');
                        for(int i = 0;i<datas.Length;i++)
                        {
                            Var[i] = int.Parse(datas[i]);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // 向用户显示出错消息
                Console.WriteLine("The file could not be read:");
            }

            Paint_Function(Laser_Style.Point);
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


        int[] Var = new int[6300];
        /// <summary>
        /// 显示画面
        /// </summary>
        /// <param name="style">false:显示:Line,false:显示:点</param>
        public void Paint_Function(Laser_Style laserstyle)
        {
            try
            {
                if (Var.Length < 6300)
                {
                    return;
                }
            }
            catch (Exception)
            {
                //throw;
                return;
            }

            int[] data = new int[Var.Length];

            float length;
            float theta;
            float[] lengthX = new float[6300];
            float[] lengthY = new float[6300];
            //定义窗体大小
            int halfWidth = this.Width / 2;
            int halfHeight = this.Height / 2;
            Console.WriteLine("Paint窗口的X坐标：" + halfWidth);
            Console.WriteLine("Paint窗口的X坐标：" + halfHeight);
            Console.WriteLine("接收到的数据长度 ：" + data.Length);

            Graphics g = CreateGraphics();
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = (int)(Var[i] * addsd);
            }


            for (int i = 0; i < 6300; i++)      //800  813
            {
                length = data[i];
                theta = (float)(i * 4 / 70 + 90);                     //(float)((i / 3) - 45);
                lengthX[i] = length * (float)(Math.Cos(Math.PI * theta / 180));
                lengthY[i] = length * (float)(Math.Sin(Math.PI * theta / 180));
            }

            g.Clear(this.BackColor);   //清除画面
            Pen laser = new Pen(Color.Red, 1);

            switch (laserstyle)
            {
                case Laser_Style.Point:
                    Point[] point = new Point[6300];
                    for (int i = 0; i < 6300; i++)      //800  813
                    {
                        point[i].X = (int)(halfWidth + lengthX[i]);
                        point[i].Y = (int)(halfHeight - lengthY[i]);
                    }
                    g.DrawLines(laser, point);
                    break;
                case Laser_Style.Line:
                    for (int i = 0; i < 6300; i++)      //800  813   //下面是划线，有延时
                    {
                        g.DrawLine(laser, halfWidth, halfHeight, halfWidth + lengthX[i], halfHeight - lengthY[i]);
                    }
                    break;
                default:
                    break;
            }
        }

        int[] LaserAngleAndLength = new int[6300];
        private void button9_Click(object sender, EventArgs e)
        {
            success_count = 0;
            for (int i = 0;i<Var.Length;i++)
            {
                LaserAngleAndLength[i] = Var[i];
            }
            changeAxisCoordinateToRectangularCoordinate(LaserAngleAndLength);

            for (int i = 0; i < 90; i++)
            {
                angelSplit(4*i, 4*i+4);
            }
            richTextBox1.Text += string.Format("success_count:{0}\n", success_count);
        }

        float length;
        float theta;
        float[] lengthX = new float[6300];
        float[] lengthY = new float[6300];
        public void changeAxisCoordinateToRectangularCoordinate(int[] data)
        {
            for (int i = 0; i < 6300; i++)      //800  813
            {
                length = data[i];
                theta = (float)(i * 4 / 70 + 90);                     //(float)((i / 3) - 45);
                lengthX[i] = length * (float)(Math.Cos(Math.PI * theta / 180));
                lengthY[i] = length * (float)(Math.Sin(Math.PI * theta / 180));
            };
        }


        /// <summary>
        /// 尝试5度一个区间  360度共6300个数据，每一度的数据是17.5个
        /// </summary>
        /// <param name="Angledelta"></param>
        public void angelSplit(int StartAngle,int EndAngle)
        {
            int startCount;
            int endCount;
            float theta_temp;
            Point[] selectPoint = new Point[100];
            List<int> index_ = new List<int>();


            //数据的范围角度换计数值
            selectPoint.Initialize();
            index_.Clear();
            startCount = (int)Math.Ceiling(StartAngle * 17.5);
            endCount = (int)Math.Ceiling(EndAngle * 17.5);
            for (int i = startCount; i < endCount; i++)
            {
                if (LaserAngleAndLength[i] > 25000)
                {
                    return;
                }
            }

            int count_temp = 0;
            int index;
            for (int i = StartAngle; i < EndAngle; i++)   //  数据抽选
            {
                index = (int)Math.Ceiling(i * 17.5);
                selectPoint[count_temp].X = (int)lengthX[index];
                selectPoint[count_temp].Y = (int)lengthY[index];
                count_temp++;
            }

            Point[] point_select = new Point[count_temp];
            for (int i = 0; i < count_temp; i++)   //  边缘检测
            {
                point_select[i].X = selectPoint[i].X;
                point_select[i].Y = selectPoint[i].Y;
            }
            line_struct a = lineCalculate(point_select);
            richTextBox1.Text += string.Format("RCA:{0}\n", a.RCA);
            richTextBox1.Text += string.Format("RCB:{0}\n", a.RCB);
            richTextBox1.Text += string.Format("Angle:{0}\n", a.angle);

            Point startPoint = new Point();
            Point endPoint = new Point();
            startPoint = getFootOfPerpendicular((int)point_select[0].X, (int)point_select[0].Y, a.RCB, -1, a.RCA);
            endPoint = getFootOfPerpendicular((int)point_select[count_temp - 1].X, (int)point_select[count_temp - 1].Y, a.RCB, -1, a.RCA);

            success_count++;
            g.DrawLine(outLine, halfWidth + startPoint.X * addsd, halfHeight - startPoint.Y * addsd, halfWidth + endPoint.X * addsd, halfHeight - endPoint.Y * addsd);

            for (int i = 0; i < count_temp; i++)
            {
                double distance = getDistanceFormPointToLine((int)point_select[i].X, (int)point_select[i].Y, a.RCB, -1, a.RCA);
                richTextBox1.Text += string.Format("point_select:{0}的直线距离是：{1}\n", i, distance);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            success_count = 0;
            int start_angle = 0;
            int max_angle = 360;

            for (int i = 0; i < Var.Length; i++)
            {
                LaserAngleAndLength[i] = Var[i];
            }
            changeAxisCoordinateToRectangularCoordinate(LaserAngleAndLength);

            while (start_angle <= 356)
            {
                start_angle = angelSplit_explane(start_angle, start_angle + 4);
            }  
            richTextBox1.Text += string.Format("success_count:{0}\n", success_count);
        }

        /// <summary>
        /// 尝试5度一个区间  360度共6300个数据，每一度的数据是17.5个
        /// </summary>
        /// <param name="Angledelta"></param>
        public int angelSplit_explane(int StartAngle, int EndAngle)
        {
            int startCount;
            int endCount;
            float theta_temp;
            Point[] selectPoint = new Point[100];
            List<int> index_ = new List<int>();

            int next_startAngle = EndAngle;

            //数据的范围角度换计数值
            selectPoint.Initialize();
            index_.Clear();
            startCount = (int)Math.Ceiling(StartAngle * 17.5);
            endCount = (int)Math.Ceiling(EndAngle * 17.5);
            for (int i = startCount; i < endCount; i++)
            {
                if (LaserAngleAndLength[i] == 30000)
                {
                    return next_startAngle;
                }
            }
            //************************************//
            int count_temp = 0;
            int index;
            for (int i = StartAngle; i < EndAngle; i++)   //  数据抽选
            {
                index = (int)Math.Ceiling(i * 17.5);
                selectPoint[count_temp].X = (int)lengthX[index];
                selectPoint[count_temp].Y = (int)lengthY[index];
                count_temp++;
            }

            Point[] point_select = new Point[count_temp];
            for (int i = 0; i < count_temp; i++)   //  边缘检测
            {
                point_select[i].X = selectPoint[i].X;
                point_select[i].Y = selectPoint[i].Y;
            }
            line_struct a = lineCalculate(point_select);
            richTextBox1.Text += string.Format("RCA:{0}\n", a.RCA);
            richTextBox1.Text += string.Format("RCB:{0}\n", a.RCB);
            richTextBox1.Text += string.Format("Angle:{0}\n", a.angle);
            //*****************************************//

            int detect_nextAngle = EndAngle;
            while (detect_nextAngle < 359)
            {
                detect_nextAngle++;
                count_temp++;
                index = (int)Math.Ceiling(detect_nextAngle * 17.5);
                selectPoint[count_temp].X = (int)lengthX[index];
                selectPoint[count_temp].Y = (int)lengthY[index];
                
                double distance = getDistanceFormPointToLine(selectPoint[count_temp].X, selectPoint[count_temp].Y, a.RCB, -1, a.RCA);

                if (distance > 100)
                {
                    detect_nextAngle--;
                    count_temp--;
                    break;
                }

            }
            next_startAngle = detect_nextAngle;

            //************************************//
            count_temp = 0;
            point_select.Initialize();
            for (int i = StartAngle; i < next_startAngle; i++)   //  数据抽选
            {
                index = (int)Math.Ceiling(i * 17.5);
                selectPoint[count_temp].X = (int)lengthX[index];
                selectPoint[count_temp].Y = (int)lengthY[index];
                count_temp++;
            }

            point_select = new Point[count_temp];
            for (int i = 0; i < count_temp; i++)   //  边缘检测
            {
                point_select[i].X = selectPoint[i].X;
                point_select[i].Y = selectPoint[i].Y;
            }
            a = lineCalculate(point_select);
            richTextBox1.Text += string.Format("RCA:{0}\n", a.RCA);
            richTextBox1.Text += string.Format("RCB:{0}\n", a.RCB);
            richTextBox1.Text += string.Format("Angle:{0}\n", a.angle);
            //*****************************************//



            Point startPoint = new Point();
            Point endPoint = new Point();
            startPoint = getFootOfPerpendicular((int)point_select[0].X, (int)point_select[0].Y, a.RCB, -1, a.RCA);
            endPoint = getFootOfPerpendicular((int)point_select[count_temp - 1].X, (int)point_select[count_temp - 1].Y, a.RCB, -1, a.RCA);

            success_count++;
            g.DrawLine(outLine, halfWidth + startPoint.X * addsd, halfHeight - startPoint.Y * addsd, halfWidth + endPoint.X * addsd, halfHeight - endPoint.Y * addsd);

            for (int i = 0; i < count_temp; i++)
            {
                double distance = getDistanceFormPointToLine((int)point_select[i].X, (int)point_select[i].Y, a.RCB, -1, a.RCA);
                richTextBox1.Text += string.Format("point_select:{0}的直线距离是：{1}\n", i, distance);
            }

            if (success_count == 10)
            {
                ;
            }
            Thread.Sleep(300);

            return next_startAngle;
        }
    }
}
