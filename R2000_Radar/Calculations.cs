using System;
using R2000_Library;



class Calculations
{
    public static double width;
    /********************************************

        Useful Data:

            int[]   Var.measurementdata
            int []  Var.angulardata
            int []  Var.background
            int     Var.numscanpoints


        *******************************************/

    public void Width()
    {
        int backgroundbuffer = 50;
        int objectbuffer = 30;
        int[] objectdistance = new int[2];
        double[] objectangle = new double[2];
        bool leadingedge = false;
        int[] objectindex = new int[2];
        width = 0;

        //Console.WriteLine("Calculating Width");
        int i;

        // Removing all null values from Var.measurementdata
        int[] filterdata = new int[Var.measurmentdata.Length];
        for (i = 0; i < Var.numscanpoints; i++)
        {
            if (Var.measurmentdata[i] > Var.maxrange || Var.measurmentdata[i] == -1)
            {
                filterdata[i] = Var.maxrange;
            }
            else
            {
                filterdata[i] = Var.measurmentdata[i];
            }

        }
        //Console.WriteLine("\r\nfilterdata = \r\n" + string.Join(" ",filterdata));

        // Find the first leading edge when compared to background
        for (i = 0; i < Var.numscanpoints; i++)
        {
            if (filterdata[i] <= Var.background[i] - backgroundbuffer)
            {
                //Console.WriteLine("Object Found0");
                objectindex[0] = i;
                leadingedge = true;

                if (i == 0)
                {
                    //Console.WriteLine("False");
                    leadingedge = false;
                }
                break;
            }
            else
            {
                //Console.WriteLine("Nothing Detected0");
                if (i == Var.numscanpoints)
                {
                    //Console.WriteLine("No Object Found0");
                    leadingedge = false;
                    break;
                }
            }
        }

        // Find trailing edge when compared to background
        bool trailingedge = false;
        if (leadingedge == true)
        {
            int j = i + 1;
            for (i = j; i < Var.numscanpoints; i++)
            {
                if (filterdata[i] >= Var.background[i] - backgroundbuffer)
                {
                    //Console.WriteLine("Object Found1");
                    objectindex[1] = i;
                    trailingedge = true;

                    if (i == Var.numscanpoints - 1 || i == j)
                    {
                        //Console.WriteLine("False");
                        trailingedge = false;

                    }
                    break;
                }
                else
                {
                    trailingedge = false;
                }
                //Console.Write(" " + i);
            }
        }

        if (leadingedge == true && trailingedge == true)
        {
            /*
            Console.WriteLine("Object Distance Leading Edge = " + objectdistance[0]);
            Console.WriteLine("Object Angle Leading Edge = " + objectangle[0]);
            Console.WriteLine("Object Distance Trailing Edge = " + objectdistance[1]);
            Console.WriteLine("Object Angle Trailing Edge = " + objectangle[1]);
            */

            // Find the midpoint of the object, distance
            int profilesize = (objectindex[1] - objectindex[0]);

            int[] profiledistance = new int[profilesize];
            double[] profileangle = new double[profilesize];
            int[] midpoints = new int[Convert.ToInt32(Math.Ceiling(profilesize * 0.05))];




            double midpointssize = Math.Ceiling(profilesize * 0.05);
            double midpointstart = Math.Ceiling(profilesize - midpointssize) / 2;

            Array.Copy(filterdata, objectindex[0], profiledistance, 0, profilesize);
            Array.Copy(profiledistance, Convert.ToInt32(midpointstart), midpoints, 0, Convert.ToInt32(midpointssize));

            Array.Copy((Var.angulardata), objectindex[0], profileangle, 0, profilesize);


            //Console.WriteLine("Profile = \r\n" + string.Join(" ", profile));
            //Console.WriteLine("Midpoints = \r\n" + string.Join(" ", midpoints));

            int sum = 0;
            for (i = 0; i < midpoints.Length; i++)
            {
                sum = sum + midpoints[i];
            }

            double averagedistance = sum / midpoints.Length;

            // Find angle

            double angle = Var.angulardata[Convert.ToInt32(((objectindex[1] - objectindex[0]) / 2) + objectindex[0])];
            //Console.WriteLine("\r\n\r\n\r\n\r\n\r\n\r\n");
            //Console.WriteLine("Distance = " + averagedistance);
            //Console.WriteLine("Angle = " + angle);

            for (i = 0; i < profiledistance.Length; i++)
            {
                if (profiledistance[i] > averagedistance + objectbuffer)
                {
                    //ignore
                }
                else
                {
                    objectdistance[0] = profiledistance[i];
                    objectangle[0] = profileangle[i];
                    break;
                }
            }

            for (i = profiledistance.Length - 1; i > 0; i--)
            {
                if (profiledistance[i] > averagedistance + objectbuffer)
                {
                    //ignore
                }
                else
                {
                    objectdistance[1] = profiledistance[i];
                    objectangle[1] = profileangle[i];
                    break;
                }
            }

            //Console.WriteLine("Object Distance Leading Edge = " + objectdistance[0]);
            //Console.WriteLine("Object Angle Leading Edge = " + objectangle[0]);
            //Console.WriteLine("Object Distance Trailing Edge = " + objectdistance[1]);
            //Console.WriteLine("Object Angle Trailing Edge = " + objectangle[1]);

            double[] radianangle = new double[2];
            radianangle[0] = Math.PI * objectangle[0] / 180;
            radianangle[1] = Math.PI * objectangle[1] / 180;


            width = Math.Round(Math.Sqrt((Math.Pow(objectdistance[0], 2)) + (Math.Pow(objectdistance[1], 2)) - (2 * objectdistance[0] * objectdistance[1]) * (Math.Cos(radianangle[0] - radianangle[1]))), 2);
            //Console.WriteLine("Width = " + width);

        }





    }







}

