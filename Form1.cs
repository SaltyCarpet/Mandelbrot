using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using System.Threading;
using ILGPU;
using ILGPU.Runtime;
using ILGPU.AtomicOperations;
using System.Timers;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Mandelbrot1._1
{
    public partial class Form1 : Form
    {

        //https://www.techotopia.com/index.php/Using_Bitmaps_for_Persistent_Graphics_in_C_Sharp
        //st=Iterations/256;si=Sidelenght_Window,Resolution;div=Divs_per_Unit;tx,ty=Coordinates_Center;sc=Scale;fx,fy=Julia_Start;
        //sw=Mandelbrot/Julia;best_Result(4,?,4,-0.75,0,3,0,0,true)/(4,?,4,0,0,4,-1.75,0,false)

        const int div = 4;
        double tx = -0.75;
        double ty = 0;
        double sc = 3;
        double fx = 0;
        double fy = 0;
        bool sw = true;

        //Constants
        static Bitmap bitmap;

        MenuStrip menuStrip1;

        private ToolStripTextBox toolStripTextBox1;
        private ToolStripTextBox toolStripTextBox2;
        private ToolStripTextBox toolStripTextBox3;
        private ToolStripTextBox toolStripTextBox4;
        private ToolStripTextBox toolStripTextBox5;
        private ToolStripTextBox toolStripTextBox6;
        private ToolStripTextBox toolStripTextBox7;

        private ToolStripMenuItem mandelbrotToolStripMenuItem;
        private ToolStripMenuItem juliaToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem toolStripMenuItem6;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripMenuItem toolStripMenuItem8;
        private ToolStripMenuItem toolStripMenuItem9;
        private ToolStripMenuItem toolStripMenuItem10;

        // Initialize ILGPU.
        private static Context context = Context.CreateDefault();
        private static Accelerator accelerator = context.GetPreferredDevice(preferCPU: false).CreateAccelerator(context);

        // load / precompile the kernel
        private static Action<Index2D, int, int, double, double, double, double, int, double, double, ArrayView<int>> loadedKernel =
            accelerator.LoadAutoGroupedStreamKernel<Index2D, int, int, double, double, double, double, int, double, double, ArrayView<int>>(Kernel);

        public Form1()
        {
            Graphics graphicsObj;
            //Size = new Size(si, si);
            bitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height,
               System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            graphicsObj = Graphics.FromImage(bitmap);
            graphicsObj.Clear(Color.White);
            graphicsObj.Dispose();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MaximumSize = new Size(si, si);
            //MinimumSize = new Size(1920, 1080);

            menuStrip1 = new MenuStrip();

            toolStripTextBox1 = new ToolStripTextBox();
            toolStripTextBox2 = new ToolStripTextBox();
            toolStripTextBox3 = new ToolStripTextBox();
            toolStripTextBox4 = new ToolStripTextBox();
            toolStripTextBox5 = new ToolStripTextBox();
            toolStripTextBox6 = new ToolStripTextBox();
            toolStripTextBox7 = new ToolStripTextBox();

            mandelbrotToolStripMenuItem = new ToolStripMenuItem();
            juliaToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            toolStripMenuItem4 = new ToolStripMenuItem();
            toolStripMenuItem5 = new ToolStripMenuItem();
            toolStripMenuItem6 = new ToolStripMenuItem();
            toolStripMenuItem7 = new ToolStripMenuItem();
            toolStripMenuItem8 = new ToolStripMenuItem();
            toolStripMenuItem9 = new ToolStripMenuItem();
            toolStripMenuItem10 = new ToolStripMenuItem();

            toolStripTextBox1.Text = "-0,75";
            toolStripTextBox2.Text = "0";
            toolStripTextBox3.Text = "3";
            toolStripTextBox4.Text = "0";
            toolStripTextBox5.Text = "0";
            toolStripTextBox6.Text = "0";
            toolStripTextBox7.Text = "0";
            mandelbrotToolStripMenuItem.Text = "Mandelbrot";
            juliaToolStripMenuItem.Text = "Julia";
            toolStripMenuItem1.Text = "Center";
            toolStripMenuItem2.Text = "X:";
            toolStripMenuItem3.Text = "Y:";
            toolStripMenuItem4.Text = "Scale:";
            toolStripMenuItem5.Text = "Start";
            toolStripMenuItem6.Text = "X:";
            toolStripMenuItem7.Text = "Y:";
            toolStripMenuItem8.Text = "Mouse";
            toolStripMenuItem9.Text = "X:";
            toolStripMenuItem10.Text = "Y:";

            toolStripTextBox1.AutoSize = true;
            toolStripTextBox2.AutoSize = true;
            toolStripTextBox3.AutoSize = true;
            toolStripTextBox4.AutoSize = true;
            toolStripTextBox5.AutoSize = true;
            toolStripTextBox6.AutoSize = true;
            toolStripTextBox7.AutoSize = true;
            mandelbrotToolStripMenuItem.AutoSize = true;
            juliaToolStripMenuItem.AutoSize = true;
            toolStripMenuItem1.AutoSize = true;
            toolStripMenuItem2.AutoSize = true;
            toolStripMenuItem3.AutoSize = true;
            toolStripMenuItem4.AutoSize = true;
            toolStripMenuItem5.AutoSize = true;
            toolStripMenuItem6.AutoSize = true;
            toolStripMenuItem7.AutoSize = true;
            toolStripMenuItem8.AutoSize = true;
            toolStripMenuItem9.AutoSize = true;
            toolStripMenuItem10.AutoSize = true;

            toolStripTextBox6.ReadOnly = true;
            toolStripTextBox7.ReadOnly = true;

            Controls.Add(menuStrip1);
            menuStrip1.Update();
            menuStrip1.Items.Add(mandelbrotToolStripMenuItem);
            menuStrip1.Items.Add(juliaToolStripMenuItem);
            menuStrip1.Items.Add(toolStripMenuItem1);
            menuStrip1.Items.Add(toolStripMenuItem2);
            menuStrip1.Items.Add(toolStripTextBox1);
            menuStrip1.Items.Add(toolStripMenuItem3);
            menuStrip1.Items.Add(toolStripTextBox2);
            menuStrip1.Items.Add(toolStripMenuItem4);
            menuStrip1.Items.Add(toolStripTextBox3);
            menuStrip1.Items.Add(toolStripMenuItem5);
            menuStrip1.Items.Add(toolStripMenuItem6);
            menuStrip1.Items.Add(toolStripTextBox4);
            menuStrip1.Items.Add(toolStripMenuItem7);
            menuStrip1.Items.Add(toolStripTextBox5);
            menuStrip1.Items.Add(toolStripMenuItem8);
            menuStrip1.Items.Add(toolStripMenuItem9);
            menuStrip1.Items.Add(toolStripTextBox6);
            menuStrip1.Items.Add(toolStripMenuItem10);
            menuStrip1.Items.Add(toolStripTextBox7);

            Paint += new PaintEventHandler(Form1_Paint);
            MouseDown += new MouseEventHandler(Form1_MouseDown);
            KeyDown += new KeyEventHandler(Form1_KeyDown);
            KeyDown += new KeyEventHandler(menuStrip1_KeyDown);
            mandelbrotToolStripMenuItem.Click += new System.EventHandler(mandelbrotToolStripMenuItem_Click);
            juliaToolStripMenuItem.Click += new System.EventHandler(juliaToolStripMenuItem_Click);
            toolStripMenuItem1.Click += new System.EventHandler(toolStripMenuItem1_Click);
            toolStripMenuItem5.Click += new System.EventHandler(toolStripMenuItem5_Click);
            toolStripMenuItem8.Click += new System.EventHandler(toolStripMenuItem8_Click);
            FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            accelerator.Dispose();
            context.Dispose();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphicsObj = e.Graphics;

            graphicsObj.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);

            graphicsObj.Dispose();
        }

        private void mandelbrotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sw = true;
            Execute();
        }

        private void juliaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sw = false;
            Execute();
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            toolStripTextBox1.Text = "0";
            toolStripTextBox2.Text = "0";
            toolStripTextBox3.Text = "3";
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            toolStripTextBox4.Text = toolStripTextBox1.Text;
            toolStripTextBox5.Text = toolStripTextBox2.Text;
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            toolStripTextBox1.Text = toolStripTextBox6.Text;
            toolStripTextBox2.Text = toolStripTextBox7.Text;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            toolStripTextBox6.Text = Convert.ToString(((Cursor.Position.X - Location.X - 9) * sc / ClientRectangle.Height - sc * ClientRectangle.Width / (2 * ClientRectangle.Height) + tx));
            toolStripTextBox7.Text = Convert.ToString((-(Cursor.Position.Y - Location.Y - 38) * sc / ClientRectangle.Height + sc / 2 + ty));
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Execute();
            }
        }

        private void menuStrip1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Execute();
            }
        }

        private void Execute()
        {
            try
            {
                tx = Convert.ToDouble(toolStripTextBox1.Text);
                ty = Convert.ToDouble(toolStripTextBox2.Text);
                sc = Convert.ToDouble(toolStripTextBox3.Text);
                fx = Convert.ToDouble(toolStripTextBox4.Text);
                fy = Convert.ToDouble(toolStripTextBox5.Text);
            }
            catch
            {
                MessageBox.Show("Falsche Eingabe");
                return;
            }
            Cursor = Cursors.AppStarting;
            //Cursor = Cursors.WaitCursor;
            Graphics graphicsObj;
            //Size = new Size(si, si);
            bitmap = new Bitmap(ClientRectangle.Width, ClientRectangle.Height,
               System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            graphicsObj = Graphics.FromImage(bitmap);
            graphicsObj.Clear(Color.White);
            //Complex c = Complex.Zero;
            //Complex z = Complex.Zero;
            double wmin = (tx - sc * ClientRectangle.Width / (2 * ClientRectangle.Height));
            double wmax = (tx + sc * ClientRectangle.Width / (2 * ClientRectangle.Height));
            double hmin = (ty - sc / 2);
            double hmax = (ty + sc / 2);
            int swint = 0;
            if (sw)
            {
                swint = 1;
            }

            // Load the data. MemoryBuffer1D<int, Stride1D.Dense>
            var deviceOutput = accelerator.Allocate1D<int>(ClientRectangle.Width * ClientRectangle.Height);

            // finish compiling and tell the accelerator to start computing the kernel
            loadedKernel(new Index2D(ClientRectangle.Width, ClientRectangle.Height), ClientRectangle.Height, ClientRectangle.Width, wmin, wmax, hmin, hmax, swint, fx, fy, deviceOutput.View);

            // wait for the accelerator to be finished with whatever it's doing
            // in this case it just waits for the kernel to finish.
            accelerator.Synchronize();

            // moved output data from the GPU to the CPU for output to console
            int[] hostOutput = deviceOutput.GetAsArray1D();
            display(hostOutput, 0, 0);

            /*            var data = bitmap.LockBits(new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height),ImageLockMode.ReadWrite,bitmap.PixelFormat);
                        var depth = Bitmap.GetPixelFormatSize(data.PixelFormat) / 8;
                        var buffer = new byte[data.Width*data.Height*depth];
                        int it = buffer.Length;
                        Marshal.Copy(data.Scan0,buffer,0,buffer.Length);

                        AutoResetEvent ready = new AutoResetEvent(false);
                        for (int i = 0; i < ClientRectangle.Height; i++)
                        {
                             // Anfordern eines Threads aus dem Pool
                             ThreadPool.QueueUserWorkItem(new WaitCallback(state => display(buffer,hostOutput.Skip(i * ClientRectangle.Width - 1).Take(ClientRectangle.Width - 1).ToArray(), i*ClientRectangle.Width-1, depth)), ready);           
                        }
                        //while (ThreadPool.PendingWorkItemCount != 0) { }
                        Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);
                        bitmap.UnlockBits(data);*/
            Pen myPen = new Pen(Color.FromArgb(100, 130, 130, 130), 1);
            try
            {
                Rectangle rectangleObj = new Rectangle((int)(-wmin * ClientRectangle.Width / (wmax - wmin)), 0, 1, ClientRectangle.Height);
                graphicsObj.DrawRectangle(myPen, rectangleObj);
                Rectangle rectangleObj1 = new Rectangle(0, (int)(ClientRectangle.Height * hmax / (hmax - hmin)), ClientRectangle.Width, 1);
                graphicsObj.DrawRectangle(myPen, rectangleObj1);
                for (int i = (int)wmin * div; i <= (int)wmax * div; i++)
                {
                    Rectangle rectangleObj3 = new Rectangle((int)((i) * ClientRectangle.Width / ((wmax - wmin) * div)) + (int)(-wmin * ClientRectangle.Width / (wmax - wmin)), -2 + (int)(ClientRectangle.Height * hmax / (hmax - hmin)), 1, 5);
                    graphicsObj.DrawRectangle(myPen, rectangleObj3);
                }
                for (int i = (int)hmin * div; i <= (int)hmax * div; i++)
                {
                    Rectangle rectangleObj4 = new Rectangle(-2 + (int)(ClientRectangle.Width * -wmin / (wmax - wmin)), -(int)((i) * ClientRectangle.Height / ((hmax - hmin) * div)) + (int)(hmax * ClientRectangle.Height / (hmax - hmin)), 6, 1);
                    graphicsObj.DrawRectangle(myPen, rectangleObj4);
                }
            }
            catch
            {

            }
            Rectangle rectangleObj2 = new Rectangle(-1 + ClientRectangle.Width / 2, -1 + ClientRectangle.Height / 2, 3, 3);
            graphicsObj.DrawRectangle(myPen, rectangleObj2);
            graphicsObj.Dispose();
            Refresh();
            Cursor = Cursors.Default;
        }

        private void display(int[] hostoutput, int offset, int depth)
        {
            for (int i = 0; i < hostoutput.Length; i++)
            {
                int it = 0;
                it = hostoutput[i];
                //it = (int)(255 - Math.Pow(0.01 * l / st, 1 / (0.01 * l / st)) * 256 / Math.Pow(Math.E, 1 / Math.E));
                bitmap.SetPixel(i / ClientRectangle.Height, i % ClientRectangle.Height, Color.FromArgb((it % 64) * 4, (it % 16) * 16, it % 256));//purple
                //bitmap.SetPixel(i, j, Color.FromArgb(it, it, it));//gray
                /*             buffer[depth * (offset + i)] = (byte)((it % 64) * 4);
                             buffer[depth * (offset + i) + 1] = (byte)((it % 16) * 16);
                             buffer[depth * (offset + i) + 2] = (byte)((it % 256));*/
            }
        }

        static void Kernel(Index2D index, int imageHeight, int imageWidth, double wmin, double wmax, double hmin, double hmax, int swint, double fx, double fy, ArrayView<int> output)
        {
            double[] c = new double[2];
            double[] z = new double[2];
            double zt = 0;
            double d = 0;
            int st = 64;
            int it = 0;
            double x = ((((wmax - wmin) / imageWidth)) * (double)index.X + wmin);
            double y = (hmax - ((hmax - hmin) / (imageHeight)) * (double)index.Y);

            if (swint == 1)
            {
                c[0] = x;
                c[1] = y;
            }
            else
            {
                z[0] = x;
                z[1] = y;
                c[0] = fx;
                c[1] = fy;
            }

            for (it = st; d <= 4 && it <= st * 256; it++)
            {
                zt = z[0];
                z[0] = (z[0] * z[0]) - (z[1] * z[1]);
                z[1] = 2 * zt * z[1];
                z[0] = z[0] + c[0];
                z[1] = z[1] + c[1];
                d = (z[0] * z[0]) + (z[1] * z[1]);
            }
            it = 256 - it / st;
            output[index.X * imageHeight + index.Y] = it;
        }
    }
}