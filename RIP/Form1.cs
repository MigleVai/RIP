using System;
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

        Graphics graphDrawingArea;
        Bitmap bmpDrawingArea;
        Graphics graph;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bmpDrawingArea = new Bitmap(Width, Height);
            graphDrawingArea = Graphics.FromImage(bmpDrawingArea);
            graph = Graphics.FromHwnd(Handle);
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
            graph.DrawString(line.Value.ToString(), new Font("Times New Roman", 13), Brushes.Black, new PointF((_first.XCord + _second.XCord)/2, (_first.YCord + _second.YCord)/2));
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

        public int BellmanFord(char source, char dest)
        {
            int verticesCount = _allPoints.Get().Count;
            int edgesCount = _allLines.Get().Count;
            int[] distance = new int[verticesCount];
            int[] distanceOther = new int[verticesCount];
            char[] names = new char[verticesCount];

            for (int i = 0; i < verticesCount; i++)
            {
                names[i] = _allPoints.Get().ElementAt(i).Name;
                distance[i] = int.MaxValue;
                distanceOther[i] = int.MaxValue;
            }

            var sourceNumber = Array.IndexOf(names, source);
            distance[sourceNumber] = 0;
            distanceOther[sourceNumber] = 0;


            for (int i = 1; i <= verticesCount - 1; ++i)//points
            {
                for (int j = 0; j < edgesCount; ++j)//lines
                {
                    var tempF = _allLines.Get().ElementAt(j).FirstName;
                    int u = Array.IndexOf(names, tempF);// first 
                    var tempS = _allLines.Get().ElementAt(j).SecondName;
                    int v = Array.IndexOf(names, tempS); //second
                    int weight = _allLines.Get().ElementAt(j).Value;

                    if (distance[u] != int.MaxValue && distance[u] + weight < distance[v])
                        distance[v] = distance[u] + weight;
                }

                for (int z = 0; z < edgesCount; ++z)//lines
                {
                    var tempS = _allLines.Get().ElementAt(z).SecondName;
                    int u = Array.IndexOf(names, tempS);// second
                    var tempF = _allLines.Get().ElementAt(z).FirstName;
                    int v = Array.IndexOf(names, tempF); //first
                    int weight = _allLines.Get().ElementAt(z).Value;

                    if (distanceOther[u] != int.MaxValue && distanceOther[u] + weight < distanceOther[v])
                        distanceOther[v] = distanceOther[u] + weight;
                }
            }

            for (int i = 0; i < edgesCount; ++i)
            {
                var tempF = _allLines.Get().ElementAt(i).FirstName;
                int u = Array.IndexOf(names, tempF);// first 
                var tempS = _allLines.Get().ElementAt(i).SecondName;
                int v = Array.IndexOf(names, tempS); //second
                int weight = _allLines.Get().ElementAt(i).Value;

                if (distance[u] != int.MaxValue && distance[u] + weight < distance[v])
                    return 404;
            }

            for (int i = 0; i < edgesCount; ++i)
            {
                var tempS = _allLines.Get().ElementAt(i).SecondName;
                int u = Array.IndexOf(names, tempS); //second
                var tempF = _allLines.Get().ElementAt(i).FirstName;
                int v = Array.IndexOf(names, tempF);// first 

                int weight = _allLines.Get().ElementAt(i).Value;

                if (distanceOther[u] != int.MaxValue && distanceOther[u] + weight < distanceOther[v])
                    return 404;
            }

            var temp = Array.IndexOf(names, dest);
            if (distance[temp] == Int32.MaxValue)
                return distanceOther[temp];
            return distance[temp];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var source = Convert.ToChar(textBox1.Text);
            var dest = Convert.ToChar(textBox2.Text);
            var result = BellmanFord(source, dest);
            label2.Text = result.ToString();
        }
    }
}
