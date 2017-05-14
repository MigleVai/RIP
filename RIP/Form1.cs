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
            timer1.Interval = 1500; // in miliseconds
            progressBar1.Maximum = 100;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value < 100)
            {
                progressBar1.Value += 5;
            }
            else
            {
                foreach (var item in _allPoints.Get())
                {
                    BellmanFord(item.Name);
                }
                _allPoints.Get().Union(_tempPoints);
                progressBar1.Value = 0;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bmpDrawingArea = new Bitmap(Width, Height);
            graphDrawingArea = Graphics.FromImage(bmpDrawingArea);
            graph = Graphics.FromHwnd(Handle);
            label2.Text = "None";
            InitTimer();
        }

        private void Form1_Click_1(object sender, EventArgs e)
        {
            var x = ((MouseEventArgs)e).X;
            var y = ((MouseEventArgs)e).Y;
            DrawCentralCircle(x, y, 15);
            graph.DrawImage(bmpDrawingArea, 0, 0);
        }

        public void DrawPoint(int x, int y, char name)
        {
            var radius = 15;
            int start = x - radius;
            int end = y - radius;
            int diam = radius * 2;
            bmpDrawingArea = new Bitmap(Width, Height);
            graphDrawingArea = Graphics.FromImage(bmpDrawingArea);
            graphDrawingArea.DrawEllipse(new Pen(Color.Black), start, end, diam, diam);
            graphDrawingArea.DrawString(name.ToString(), new Font("Times New Roman", 13), Brushes.Black, new PointF(x - 8, y - 10));
        }

        void DrawCentralCircle(int CenterX, int CenterY, int Radius)
        {
            if (_asciiNumber >= 91) // Z => 90
                return;

            var p = new Point(CenterX, CenterY);
            p.Name = Convert.ToChar(_asciiNumber);
            _allPoints.Add(p);
            DrawPoint(CenterX, CenterY, p.Name);
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
            DrawLines(line, _first, _second);
            AddNeighbour(_first.Name);
            AddNeighbour(_second.Name);
        }

        public void DrawLines(Lines line, Point _first, Point _second)
        {
            graph.DrawString(line.Value.ToString(), new Font("Times New Roman", 13), Brushes.Black, new PointF((_first.XCord + _second.XCord) / 2, (_first.YCord + _second.YCord) / 2));
            graph.DrawLine(new Pen(Color.Black), _first.XCord, _first.YCord, _second.XCord, _second.YCord);
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
                var tempHop = point.Neighbors.Where(o => !point.Hops.Any(p => o.Key == p.Destination && o.Value == p.Value));
                if (tempHop != null)
                {
                    foreach (var item in tempHop)
                    {
                        tempP.Add(new Hop(item.Key, item.Value, item.Key));
                    }
                }
            }

            temp.Find(o => o.Name == source).Hops = tempP;
            _allPoints.Set(temp);
        }

        public void BellmanFord(char source)
        {
            var tempPoint = _allPoints.Get();
            var point = tempPoint.Find(o => o.Name == source);
            int free;
            var tempHops = point.Hops;

            var test = point.Hops.Find(o => o.Value == Int32.MaxValue);
            var listHops = point.Hops; 

            if (test != null)
            {
                var tempTest = point.Hops.Find(o => o.Value == Int32.MaxValue);
                if (tempTest != null)
                {
                    foreach (var itemP in point.Neighbors)
                    {
                        if (itemP.Key != tempTest.Destination && itemP.Key.Hops.Exists(o => o.Destination == tempTest.Destination || o.HopStep == tempTest.HopStep))
                        {
                            itemP.Key.Hops.Find(o => o.Destination == tempTest.Destination || o.HopStep == tempTest.HopStep).Value = Int32.MaxValue;
                        }
                    }
                    point.Neighbors[tempTest.Destination] = Int32.MaxValue;
                    foreach (var itemH in listHops)
                    {
                        if (itemH.Destination == tempTest.Destination || itemH.HopStep == tempTest.Destination)
                        {
                            var index = point.Hops.IndexOf(itemH);
                            point.Hops[index].Value = Int32.MaxValue;
                        }
                    }
                }
            }

            var count = point.Hops.RemoveAll(o => o.Value == Int32.MaxValue);
            if (count != 0)
            {
                var all = point.Neighbors.First(o => o.Value == Int32.MaxValue);
                point.Neighbors.Remove(all.Key);
                point.Hops.Clear();
                foreach (var item in point.Neighbors)
                    point.Hops.Add(new Hop(item.Key, item.Value, item.Key));
            }

            for (int i = 1; i < tempPoint.Count - 1; i++)
            {
                foreach (var item in tempPoint)
                {
                    if (item.Hops.Exists(o => o.Destination.Name == source))
                    {
                        free = item.Hops.Find(o => o.Destination.Name == source).Value;
                        if (point.Neighbors.Any(o => o.Key == item))
                        {
                            foreach (var pItem in item.Hops)
                            {
                                if (pItem.Value != Int32.MaxValue)
                                {
                                    if (pItem.Destination.Name != source)
                                    {
                                        var temp = point.Hops.Find(o => o.Destination == pItem.Destination);
                                        if (temp != null)
                                        {
                                            if (free + pItem.Value < temp.Value)
                                            {
                                                var tempP = point.Hops.Find(o => o.Destination == pItem.Destination);
                                                var index = point.Hops.IndexOf(tempP);
                                                point.Hops[index] = new Hop(pItem.Destination, free + pItem.Value, item);
                                                _tempPoints.Add(point);
                                            }
                                        }
                                        else
                                        {
                                            point.Hops.Add(new Hop(pItem.Destination, free + pItem.Value, item));
                                            _tempPoints.Add(point);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
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
            var sum = 0;

            if (!_allPoints.Get().Exists(o => o == last) || !_allPoints.Get().Exists(o => o == first))
            {
                MessageBox.Show("Point does not exist!", "Warning", MessageBoxButtons.OK);
                return;
            }

            if (!first.Hops.Any(o => o.Destination == last))
            {
                MessageBox.Show(first.Name + " does not know of " + last.Name, "Warning", MessageBoxButtons.OK);
                path = "Undefined";
            }
            else
            {
                foreach (Point item in tempPoint)
                {
                    sum = first.Hops.Find(o => o.Destination == last).Value;
                    while (first != last)
                    {
                        path += first.Name;
                        var temp = first.Hops.Find(o => o.Destination == last);
                        first = temp.HopStep;
                    }
                    path += first;
                    if (last.Name == sChar)
                        break;
                }
                path += (" " + sum);
            }
            label2.Text = path;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var name = Convert.ToChar(textBox4.Text);
            var listP = _allPoints.Get();
            var listL = new List<Lines>();
            listL.AddRange(_allLines.Get());
            var temp = listP.Find(o => o.Name == name);

            foreach (var item in temp.Neighbors)
            {
                var neighbour = _allPoints.Get().Find(o => o == item.Key);
                neighbour.Hops.Find(o => o.Destination.Name == name && o.HopStep.Name == name).Value = Int32.MaxValue;
            }

            foreach (var item in _allLines.Get())
            {
                if (item.FirstName == name)
                    listL.Remove(item);

                if (item.SecondName == name)
                    listL.Remove(item);
            }
            _allLines.Set(listL);
            _allPoints.Delete(temp);

            _allPoints.Get();
            graph.Clear(Color.WhiteSmoke);
            this.Refresh();

            foreach (var item in _allPoints.Get())
            {
                DrawPoint(item.XCord, item.YCord, item.Name);
                graph.DrawImage(bmpDrawingArea, 0, 0);
            }
            foreach (var item in _allLines.Get())
            {
                DrawLines(item, item.First, item.Second);
                graph.DrawImage(bmpDrawingArea, 0, 0);
            }
        }
    }
}
