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
        private int direction;
        private List<Point> segments;
        private Random rand;
        private Color bckOriginal;
        private bool gameon;
        private int gameoncounter;
        private int size = ConstNumbers.SegmentSize;
        private int counter = ConstNumbers.Counter;
        private int sleep = ConstNumbers.SleepInit;
        private int maxx = ConstNumbers.MaxBorderX;
        private int maxpx = ConstNumbers.MaxPointX;
        //private int maxy = ConstNumbers.MaxBorderY;
        private int maxpy = ConstNumbers.MaxPointY;
        private int minx = ConstNumbers.MinimumBorderX;
        private int minpx = ConstNumbers.MinimumPointX;
        private int miny = ConstNumbers.MinimumBorderY;
        private int minpy = ConstNumbers.MinimumPointY;
        private int clbr = ConstNumbers.CalibrateConst;

        public Form1()
        {
            rand = new Random();

            Start();

            smoothingMode = SmoothingMode.HighQuality;
            textSmoothing = TextRenderingHint.AntiAlias;
            InitializeComponent();
            bckOriginal = this.BackColor;
        }

        private void Start()
        {
            direction = ConstNumbers.STOP;
            int px = rand.Next(minpx, maxpx);
            px = px - ((px-clbr) % size);
            int py = rand.Next(minpy, maxpy);
            py = py - ((py - clbr) % size);

            Point head = new Point(px, py);
            
            int ax = rand.Next(minpx, maxpx);
            ax = ax - ((ax - clbr) % size);
            int ay = rand.Next(minpy, maxpy);
            ay = ay - ((ay-clbr) % size);
            apple = new Point(ax, ay);

            segments = new List<Point>();
            segments.Add(head);
            gameon = true;
            gameoncounter = ConstNumbers.GameCounter ;
        }

        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int ax = rand.Next(minpx, maxpx);
                ax = ax - ((ax - clbr) % size);
                int ay = rand.Next(minpy, maxpy);
                ay = ay - ((ay - clbr) % size);
                apple = new Point(ax, ay);
            }
        }

        void keyDown(object sender, KeyEventArgs e)
        {
            this.BeginInvoke(new Action(
            () =>
            {
                switch (e.KeyCode)
                {
                    case Keys.Down:
                        direction = ConstNumbers.DOWN;
                        break;
                    case Keys.Up:
                        direction = ConstNumbers.UP;
                        break;
                    case Keys.Left:
                        direction = ConstNumbers.LEFT;
                        break;
                    case Keys.Right:
                        direction = ConstNumbers.RIGHT;
                        break;
                    case Keys.Space:
                        direction = ConstNumbers.STOP;
                        break;
                }
                MoveSegments(direction);
            }));
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

        void MoveSegments(int dir)
        {
            Point tmp = segments.Last();
            switch (dir)
            {
                case ConstNumbers.DOWN:
                    tmp.Y += size;
                    break;
                case ConstNumbers.UP:
                    tmp.Y -= size;
                    break;
                case ConstNumbers.LEFT:
                    tmp.X -= size;
                    break;
                case ConstNumbers.RIGHT:
                    tmp.X += size;
                    break;
            }
            if ((tmp.Y < minpy || tmp.Y > maxpy) || (tmp.X < minpx || tmp.X > maxpx))
            {
                gameon = false;
            }
            if (direction != ConstNumbers.STOP)
            {
                segments.Remove(segments.First());
                segments.Add(tmp);
            }
        }
        

        void OnApple()
        {
            Point head = segments.Last();

            if (Math.Abs(head.X - apple.X) < size && Math.Abs(head.Y - apple.Y) < size)
            {
                //Debug.WriteLine("Apple: " +apple.X+ " " + apple.Y);
                //Debug.WriteLine("Head : " + head.X + " " + head.Y);
                Point tmp = segments.Last();
                if (direction == ConstNumbers.RIGHT)
                    tmp.X += size;
                if (direction == ConstNumbers.LEFT)
                    tmp.X -= size;
                if (direction == ConstNumbers.DOWN)
                    tmp.Y += size;
                if (direction == ConstNumbers.UP)
                    tmp.Y -= size;
                segments.Add(tmp);

                int ax = rand.Next(minpx, maxpx);
                ax = ax - ((ax - clbr) % size);
                int ay = rand.Next(minpy, maxpy);
                ay = ay - ((ay - clbr) % size);
                apple = new Point(ax, ay);

                sleep = sleep == 20 ? 20 : sleep - 1;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            Bitmap buffer;
            buffer = new Bitmap(this.Width, this.Height);
            Brush aBrush = (Brush)Brushes.Red;
            Brush aBrush1 = (Brush)Brushes.Red;
            Brush aBrush2 = (Brush)Brushes.Green;
            Brush aBrush3 = (Brush)Brushes.LightGreen;
            //start an async task
            try
            {
                counter++;
                
                using (Graphics gg = Graphics.FromImage(buffer))
                {                    
                    gg.SmoothingMode = smoothingMode;
                    gg.TextRenderingHint = textSmoothing;

                    //For debugging: shows grid
                    //for (int i = 0; i < 63; i++)
                    //{
                    //    gg.DrawLine(new Pen(Color.Gray), minx, miny + (i * 4 + 2), maxx, miny + (i * 4 + 2));
                    //    gg.DrawLine(new Pen(Color.Gray), minx + (i * 4 + 2), miny, minx + (i * 4 + 2), maxy);
                    //}
                    string score = "Score : " + segments.Count;
                    Font drawFont = new Font("Arial", 8);
                    SolidBrush drawBrush = new SolidBrush(Color.Black);
                    PointF drawPoint = new Point(5, 5);
                    gg.DrawString(score, drawFont, drawBrush, drawPoint);

                    //gg.DrawLine(new Pen(Color.Gray), minx-1, miny-1, minx-1, maxy+1);                   // Vertical left gray
                    //gg.DrawLine(new Pen(Color.Black), minx, miny, minx, maxy);                          // Vertical left black
                    gg.DrawLine(new Pen(Color.LightGray), minx - 1, miny - 2, maxx + 1, miny - 2);        // Horizontal top light gray
                    gg.DrawLine(new Pen(Color.Gray), minx-1, miny-1, maxx+1, miny-1);                     // Horizontal top gray
                    gg.DrawLine(new Pen(Color.Black), minx, miny, maxx, miny);                            // Horizontal top black
                    //gg.DrawLine(new Pen(Color.Gray), maxx + 1, miny - 1, maxx + 1, maxy + 1);           // Vertical right gray
                    //gg.DrawLine(new Pen(Color.Black), maxx, miny, maxx, maxy);                          // Vertical right black
                    //gg.DrawLine(new Pen(Color.Gray), minx-1, maxy+1, maxx+1, maxy+1);                   // Horizontal bottom gray
                    //gg.DrawLine(new Pen(Color.Black), minx, maxy, maxx, maxy);                          // Horizontal bottom black

                    gg.FillEllipse(aBrush1, new RectangleF(new Point(apple.X-2, apple.Y-2), new Size(5, 5)));
                    gg.DrawEllipse(new Pen(Color.Black), new RectangleF(new Point(apple.X - 2, apple.Y - 2), new Size(5, 5)));
                    gg.DrawEllipse(new Pen(Color.Gray), new RectangleF(new Point(apple.X - 3, apple.Y - 3), new Size(7, 7)));

                    for (int i = 0; i < segments.Count; i++)
                    {
                        Point segment = segments[i];
                        aBrush = i == segments.Count - 1 ? aBrush2 : aBrush3;
                        gg.FillEllipse(aBrush, new RectangleF(new Point(segment.X - 2, segment.Y - 2), new Size(5, 5)));
                        gg.DrawEllipse(new Pen(Color.Black), new RectangleF(new Point(segment.X - 2, segment.Y - 2), new Size(5, 5)));
                    }

                    OnApple();

                    if (counter >= sleep && gameon)
                    {
                        MoveSegments(direction);
                        counter = 0;
                    }              
                    if (!gameon)
                    {
                        this.BackColor = Color.LightPink;
                        gameoncounter--;
                    }     
                    if (gameoncounter<=0)
                    {
                        this.BackColor = bckOriginal;
                        gameon = true;
                        gameoncounter = ConstNumbers.GameCounter;
                        Start();
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
    public struct ConstNumbers
    {
        public const int SegmentSize       = 5;
        public const int Counter           = 0;
        public const int SleepInit         = 100;
        public const int MaxBorderX        = 282;
        public const int MaxPointX         = 279;
        public const int MaxBorderY        = 262;
        public const int MaxPointY         = 259;
        public const int MinimumBorderX    = 1;
        public const int MinimumPointX     = 3;
        public const int MinimumBorderY    = 25;
        public const int MinimumPointY     = 27;
        public const int CalibrateConst    = 3;
        public const int GameCounter       = 300;
        public const int LEFT              = 0;
        public const int UP                = 1;
        public const int RIGHT             = 2;
        public const int DOWN              = 3;
        public const int STOP              = 4;
    }
}
