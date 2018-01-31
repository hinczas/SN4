using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SN4
{
    public partial class Form1 : Form
    {
        private SmoothingMode smoothingMode;
        private TextRenderingHint textSmoothing;
        private Point apple;
        private char direction;
        private List<Point> segments;
        private Random rand;
        private int size= 4;
        private int counter = 0;
        private int sleep = 100;
        private int maxx = 282;
        private int maxy = 260;

        public Form1()
        {
            rand = new Random();

            Start();

            smoothingMode = SmoothingMode.HighQuality;
            textSmoothing = TextRenderingHint.SystemDefault;
            InitializeComponent();
        }

        private void Start()
        {
            direction = 'S';
            int px = rand.Next(0, maxx);
            px = px - (px % size);
            int py = rand.Next(0, maxy);
            py = py - (py % size);

            Point head = new Point(px, py);

            int ax = rand.Next(0, maxx);
            ax = ax - (ax % size);
            int ay = rand.Next(0, maxy);
            ay = ay - (ay % size);
            apple = new Point(ax, ay);

            segments = new List<Point>();
            segments.Add(head);
        }

        void keyDown(object sender, KeyEventArgs e)
        {
            this.BeginInvoke(new Action(
            () =>
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        direction = 'D';
                        break;
                    case Keys.Up:
                        direction = 'U';
                        break;
                    case Keys.Left:
                        direction = 'L';
                        break;
                    case Keys.Right:
                        direction = 'R';
                        break;
                    case Keys.Space:
                        direction = 'P';
                        break;
                }
                MoveSegments(direction);
            }));
        }

        void MoveSegments(char dir)
        {
            Point tmp = segments.Last();
            switch (dir)
            {
                case 'D':
                    tmp.Y += size;
                    break;
                case 'U':
                    tmp.Y -= size;
                    break;
                case 'L':
                    tmp.X -= size;
                    break;
                case 'R':
                    tmp.X += size;
                    break;
            }
            if ((tmp.Y < 0 || tmp.Y > maxy-size) || (tmp.X < 0 || tmp.X > maxx-size))
            {
                Start();
            }
            else
            {
                segments.Remove(segments.First());
                segments.Add(tmp);
            }
        }
        
        // PreviewKeyDown is where you preview the key.
        // Do not put any logic here, instead use the
        // KeyDown event after setting IsInputKey to true.
        private void previewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Up:
                case Keys.Left:
                case Keys.Right:
                    e.IsInputKey = true;
                    break;
            }
        }

        void OnApple()
        {
            Point head = segments.Last();

            if (Math.Abs(head.X - apple.X) < size && Math.Abs(head.Y - apple.Y) < size)
            {
                Point tmp = segments.Last();
                if (direction == 'R')
                    tmp.X += size;
                if (direction == 'L')
                    tmp.X -= size;
                if (direction == 'D')
                    tmp.Y += size;
                if (direction == 'U')
                    tmp.Y -= size;

                segments.Add(tmp);

                int ax = rand.Next(0, maxx);
                ax = ax - (ax % size);
                int ay = rand.Next(0, maxy);
                ay = ay - (ay % size);
                apple = new Point(ax, ay);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            Bitmap buffer;
            buffer = new Bitmap(this.Width, this.Height);
            //Brush aBrush = (Brush)Brushes.Red;
            //Brush aBrush1 = (Brush)Brushes.Red;
            //Brush aBrush2 = (Brush)Brushes.Green;
            //Brush aBrush3 = (Brush)Brushes.LightGreen;
            Brush aBrush = (Brush)Brushes.LightGray;
            Brush aBrush1 = (Brush)Brushes.LightGray;
            Brush aBrush2 = (Brush)Brushes.LightGray;
            Brush aBrush3 = (Brush)Brushes.LightGray;
            Pen pen = (Pen)Pens.Red;
            //start an async task
            try
            {
                counter++;
                
                using (Graphics gg = Graphics.FromImage(buffer))
                {                    
                    gg.SmoothingMode = smoothingMode;
                    gg.TextRenderingHint = textSmoothing;

                    //for (int i =0; i < 63; i++)
                    //{
                    //    gg.DrawLine(new Pen(Color.Gray), 0, i*4 + 2 , 255, i * 4 + 2);
                    //    gg.DrawLine(new Pen(Color.Gray), i * 4 + 2, 0, i * 4 + 2, 255);
                    //}
                    //gg.DrawLine(new Pen(Color.Black), maxx, 0, maxx, maxy);
                    //gg.DrawLine(new Pen(Color.Black), 0, maxy, maxx, maxy);
                    gg.FillEllipse(aBrush1, new RectangleF(apple, new Size(4, 4)));
                    gg.DrawEllipse(new Pen(Color.LightGray), new RectangleF(apple, new Size(4, 4)));

                    for (int i = 0; i < segments.Count; i++)
                    {
                        Point segment = segments[i];
                        aBrush = i == segments.Count - 1 ? aBrush2 : aBrush3;
                        gg.FillEllipse(aBrush, new RectangleF(segment, new Size(5, 5)));
                        gg.DrawEllipse(new Pen(Color.LightGray), new RectangleF(segment, new Size(5, 5)));
                    }

                    OnApple();

                    if (counter == sleep)
                    {
                        MoveSegments(direction);
                        counter = 0;
                    }                   
                }
                //invoke an action against the main thread to draw the buffer to the background image of the main form.
                if (!this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            this.BackgroundImage = buffer;
                        }));
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine(exc.Message);
                    }
                }
            }
            catch (Exception z)
            {
                Debug.WriteLine(z.Message);
            }
        }
    }
}
