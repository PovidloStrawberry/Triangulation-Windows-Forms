using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Triangulation
{
    struct edge
    {
        public Point p1;
        public Point p2;
        public edge(Point p1, Point p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    };
    public partial class Form1 : Form
    {
        private Graphics graphics;
        private List<Point> points = new List<Point>();
        private List<List<Point>> points_centers = new List<List<Point>>();
        private List<edge> edges = new List<edge>();
        private List<Color> point_Colors = new List<Color>() { Color.AliceBlue, Color.Bisque, Color.BurlyWood, Color.DarkRed };
        private Pen pen = new Pen(Brushes.Black);
        private Pen dots_Pen = new Pen(Brushes.White);
        private StreamWriter fileWriter = new StreamWriter("C:\\Users\\matve\\OneDrive\\Рабочий стол\\папки\\c# prj\\Triangulation\\text_Output.txt", true, Encoding.ASCII);
        private int point_Size
        {
            get
            {
                return point_Size;
            }
            set
            {
                if (value % 2 == 0)
                    point_Size = value;
            }
        }
        public Form1()
        {
            InitializeComponent();
            fileWriter.WriteLine(DateTime.Now);
        }

        private float Distance_Berween_Points(Point p1, Point p2)
        {
            return (float)Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }
        private void Draw_Points()
        {
            foreach (Point point in points)
            {
                graphics.FillRectangle(Brushes.Black, point.X - 5, point.Y - 5, 10, 10);
            }
            pictureBox1.Refresh();
        }
        private float Are_Edges_Crossing(edge e1, edge e2)
        {
            float x11 = e1.p1.X;
            float x12 = e1.p2.X;
            float x21 = e2.p1.X;
            float x22 = e2.p2.X;

            float y11 = e1.p1.Y;
            float y12 = e1.p2.Y;
            float y21 = e2.p1.Y;
            float y22 = e2.p2.Y;

            float a1 = y11 - y12;
            float b1 = x12 - x11;
            float c1 = x11 * y12 - x12 * y11;

            float a2 = y21 - y22;
            float b2 = x22 - x21;
            float c2 = x21 * y22 - x22 * y21;

            if (a1 * b2 - a2 * b1 == 0)
                return -1;

            float x = (b1 * c2 - b2 * c1) / (a1 * b2 - a2 * b1);

            if ( (x > x11 && x < x12 || x > x12 && x < x11) && (x > x21 && x < x22 || x > x22 && x < x21))
                return x;
            return -1;
        }
        private void Write_Edge_In_File(edge edge)
        {
            fileWriter.WriteLine($"{edge.p1.X} {edge.p1.Y}   {edge.p2.X} {edge.p2.Y}");
        }
        private bool Is_Edge_Crossed_Another(edge e)
        {
            foreach (edge edge in edges)
            {
                if (Are_Edges_Crossing(e, edge) != -1)
                {
                    //fileWriter.WriteLine($"Next edges are crossing in {Are_Edges_Crossing(e, edge)}:");
                    //fileWriter.Write(points.IndexOf(edge.p1) + " " + points.IndexOf(edge.p2) + " ");
                    //Write_Edge_In_File(edge);
                    //fileWriter.Write(points.IndexOf(e.p1) + " " + points.IndexOf(e.p2) + " ");
                    //Write_Edge_In_File(e);
                    //fileWriter.WriteLine(" ");
                    return true;
                }
            }
            return false;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (graphics != null)
            {
                int posX = PictureBox.MousePosition.X - splitContainer1.Panel1.Width - 10;
                int posY = PictureBox.MousePosition.Y - 35;

                Point new_Point = new Point(posX + 5, posY + 5);

                if (points.IndexOf(new_Point) == -1) {
                    points.Add(new_Point);
                    graphics.FillRectangle(Brushes.Black, posX, posY, 10, 10);
                    pictureBox1.Refresh();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            points.Clear();
            edges.Clear();
            pen.Width = 5;
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            graphics = Graphics.FromImage(pictureBox1.Image);
            graphics.Clear(Color.White);
            pictureBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e) // триангуляция
        {
            edges.Clear();
            if (points.Count >= 3 && graphics != null)
            {
                graphics.Clear(Color.White);
                Draw_Points();
                points.Sort((p1, p2) => (p1.X.CompareTo(p2.X)));
                graphics.DrawLine(pen, points[0], points[1]);
                graphics.DrawLine(pen, points[0], points[2]);
                graphics.DrawLine(pen, points[1], points[2]);
                edges.Add(new edge(points[0], points[1]));
                edges.Add(new edge(points[0], points[2]));
                edges.Add(new edge(points[1], points[2]));
                for (int i = 3; i < points.Count; i++)
                {
                    int j = i - 1;

                    while(j >= 0)
                    {
                        if(!Is_Edge_Crossed_Another(new edge(points[i], points[j])))
                        {
                            graphics.DrawLine(pen, points[i], points[j]);
                            edges.Add(new edge(points[i], points[j]));
                        }
                        j--;
                    }
                    /*
                    graphics.DrawLine(pen, points[i], points[j]);
                    edges.Add(new edge(points[i], points[j]));
                    j--;
                    while (j > 0 && Is_Edge_Crossed_Another(new edge(points[i], points[j])))
                    {
                        //fileWriter.WriteLine($"{i} {j} {new edge(points[i], points[j]).p1.X} {new edge(points[i], points[j]).p1.Y} {new edge(points[i], points[j]).p2.X} {new edge(points[i], points[j]).p2.Y} {Is_Edge_Crossed_Another(new edge(points[i], points[j]))}");
                        j--;
                    }
                    j = Math.Max(j, 0);
                    graphics.DrawLine(pen, points[i], points[j]);
                    edges.Add(new edge(points[i], points[j]));
                    */
                }
                pictureBox1.Refresh();
            }
            //fileWriter.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Voronov_Diagram_1();
            Voronov_Diagram_Bruteforce();
            pictureBox1.Refresh();
        }
        private void Voronov_Diagram_1() // диаграмма Вороного
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                List<Point> i_Point_Bounds = new List<Point>();
                for (int j = 0; j < points.Count - 1; j++)
                {
                    if (j + 1 != i)
                    {
                        Point current_Point_Center = new Point(points[i].X - (points[i].X - points[j + 1].X) / 2, points[i].Y - (points[i].Y - points[j + 1].Y) / 2);
                        graphics.FillRectangle(Brushes.Red, current_Point_Center.X, current_Point_Center.Y, 3, 3);
                        i_Point_Bounds.Add(current_Point_Center);
                    }
                }
                points_centers.Add(i_Point_Bounds);
            }

            for (int i = 0; i < points_centers.Count; i++)
            {
                for (int j = 0; j < points_centers[i].Count - 1; j++)
                {
                    pen.Width = 3;
                    pen.Color = Color.Red;
                    //graphics.DrawLine(pen, points_centers[i][j], points_centers[i][j + 1]);
                    pen.Color = Color.Black;
                    pen.Width = 10;
                }
            }
        }
        private void Voronov_Diagram_Bruteforce()
        {
            for (int i = 0; i < pictureBox1.Width; i++)
            {
                for (int j = 0; j < pictureBox1.Height; j++)
                {
                    Point current_Point = new Point(i, j);
                    points.Sort((p1, p2) => (Distance_Berween_Points(current_Point, p1).CompareTo(Distance_Berween_Points(current_Point, p2))));
                    if (points[0] == new Point(500, 300))
                    {
                        graphics.FillRectangle(Brushes.Red, i, j, 1, 1);
                    }
                }
            }
        }
    }
}
