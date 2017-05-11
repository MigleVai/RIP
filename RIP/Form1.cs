using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RIP
{
    public partial class Form1 : Form
    {
        private int _asciiNumber = 65; //A
        private GenericLists<Point> _allPoints = new GenericLists<Point>();
        private GenericLists<Lines> _allLines = new GenericLists<Lines>();
        private Point _first;
        private Point _second;
        private List<Point> _tempPoints = new List<Point>();

        Graphics graphDrawingArea;
        Bitmap bmpDrawingArea;
        Graphics graph;
        public Form1()
        {
            InitializeComponent();
        }

        private Timer timer1;
        public void InitTimer()
        {
            timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 60000; // in miliseconds
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label3.Text = "TICK!";
            foreach (var item in _allPoints.Get())
            {
                BellmanFord(item.Name);
            }
            _allPoints.Get().Union(_tempPoints);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bmpDrawingArea = new Bitmap(Width, Height);
            graphDrawingArea = Graphics.FromImage(bmpDrawingArea);
            graph = Graphics.FromHwnd(Handle);
            InitTimer();
        }

        private void Form1_Click_1(object sender, EventArgs e)
        {
            var x = ((MouseEventArgs)e).X;
            var y = ((MouseEventArgs)e).Y;
            DrawCentralCircle(x, y, 15);
            graph.DrawImage(bmpDrawingArea, 0, 0);
        }

        void DrawCentralCircle(int CenterX, int CenterY, int Radius)
        {
            if (_asciiNumber >= 91) // Z => 90
                return;

            var p = new Point(CenterX, CenterY);
            p.Name = Convert.ToChar(_asciiNumber);
            _allPoints.Add(p);

            int start = CenterX - Radius;
            int end = CenterY - Radius;
            int diam = Radius * 2;
            bmpDrawingArea = new Bitmap(Width, Height);
            graphDrawingArea = Graphics.FromImage(bmpDrawingArea);
            graphDrawingArea.DrawEllipse(new Pen(Color.Black), start, end, diam, diam);
            graphDrawingArea.DrawString(p.Name.ToString(), new Font("Times New Roman", 13), Brushes.Black, new PointF(CenterX - 8, CenterY - 10));
            _asciiNumber++;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(textBox2.Text) || String.IsNullOrEmpty(textBox3.Text))
                return;

            var fPoint = textBox1.Text;
            var ePoint = textBox2.Text;
            _first = Coordinates(Convert.ToChar(fPoint));
            _second = Coordinates(Convert.ToChar(ePoint));
            if (_first == null || _second == null)
                return;

            var line = new Lines(_first, _second, _first.Name, _second.Name);
            line.Value = Convert.ToInt32(textBox3.Text);
            _allLines.Add(line);
            var x = Math.Max(_first.XCord, _second.XCord);
            var y = Math.Max(_first.YCord, _second.YCord);
            graph.DrawString(line.Value.ToString(), new Font("Times New Roman", 13), Brushes.Black, new PointF((_first.XCord + _second.XCord) / 2, (_first.YCord + _second.YCord) / 2));
            graph.DrawLine(new Pen(Color.Black), _first.XCord, _first.YCord, _second.XCord, _second.YCord);
            AddNeighbour(_first.Name);
            AddNeighbour(_second.Name);
        }

        public Point Coordinates(char c)
        {
            foreach (Point p in _allPoints.Get())
            {
                if (p.Name == c)
                    return p;
            }
            return null;
        }

        public void AddNeighbour(char source)
        {
            var temp = _allPoints.Get();
            var tempLines = _allLines.Get();
            var point = _allPoints.Get().Find(o => o.Name == source);
            List<Hop> tempP = point.Hops;
            var tempDic = new Dictionary<Point, int>();
            //if (temp.Find(o => o.Name == source).Neighbors.Count == 0)
            //{
            foreach (var item in tempLines)
            {
                if (item.FirstName == source)
                    tempDic.Add(item.Second, item.Value);

                if (item.SecondName == source)
                    tempDic.Add(item.First, item.Value);
            }

            temp.Find(o => o.Name == source).Neighbors = tempDic;

            if (point.Hops.Count == 0)
            {
                foreach (var item in point.Neighbors)
                {
                    tempP.Add(new Hop(item.Key, item.Value, item.Key));
                }
            }
            else
            {
                //foreach (var p in point.Hops)
                //{
                var tempHop = point.Neighbors.Where(o => !point.Hops.Any(p => o.Key == p.Destination && o.Value == p.Value));
                if (tempHop != null)
                {
                    foreach (var item in tempHop)
                    {
                        tempP.Add(new Hop(item.Key, item.Value, item.Key));
                    }
                }
                //}
            }

            temp.Find(o => o.Name == source).Hops = tempP;
            _allPoints.Set(temp);
            //}
        }

        public void BellmanFord(char source)
        {
            var tempPoint = _allPoints.Get();
            var tempLine = _allLines.Get();
            var point = tempPoint.Find(o => o.Name == source);
            //string path = string.Empty;
            //Point first = point;
            int free;

            for (int i = 1; i < tempPoint.Count - 1; i++)
            {
                //else
                //{
                foreach (var item in tempPoint)
                {
                    if (item.Hops.Exists(o => o.Destination.Name == source))
                    {
                        free = item.Hops.Find(o => o.Destination.Name == source).Value;
                        foreach (var pItem in item.Hops)
                        {
                            if (pItem.Destination.Name != source)
                            {
                                var temp = point.Hops.Find(o => o.Destination == pItem.Destination);
                                if (temp != null)
                                {
                                    if (free + pItem.Value < temp.Value)
                                    {
                                        //var index = tempPoint.IndexOf(point);
                                        point.Hops.Find(o => o.Destination == pItem.Destination).Value = free + pItem.Value;
                                        _tempPoints.Add(point);
                                        //_allPoints.Set(tempPoint);
                                    }
                                }
                                else
                                {
                                    //var index = tempPoint.IndexOf(point);
                                    point.Hops.Add(new Hop(pItem.Destination, free + pItem.Value, item));
                                    _tempPoints.Add(point);
                                    //_allPoints.Set(tempPoint);
                                }
                            }
                        }
                    }
                }
                //}
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var fChar = Convert.ToChar(textBox1.Text);
            var sChar = Convert.ToChar(textBox2.Text);

            var tempPoint = _allPoints.Get();
            Point first = tempPoint.Find(o => o.Name == fChar);
            Point last = tempPoint.Find(o => o.Name == sChar);
            string path = string.Empty;

            foreach (Point item in tempPoint)
            {
                //foreach (var last in item.Neighbors.Keys)
                //{
                while (first != last)
                {
                    path += first.Name;
                    first = first.Hops.Find(o => o.Destination == last).HopStep;
                }
                path += first;
                if (last.Name == sChar)
                    break;
                // }
            }

            label2.Text = path;
        }
    }
}
