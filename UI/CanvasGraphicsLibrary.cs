using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UI
{
    internal class CanvasGraphicsLibrary
    {
        public static void AddPoint(Canvas canvas, double x, double y, Brush brush)
        {
            var line = new Line() { X1 = 0, Y1 = 0, X2 = 1, Y2 = 1, StrokeThickness = 1, Stroke = brush, Fill = brush };
            AddToCanvasAt(canvas, line, x, y); 
        }

        public static void AddLine(Canvas canvas, double x1, double y1, double x2, double y2, Brush brush, double thickness)
        {
            if (x1 >= 0 && y1 >= 0 && x2 >= 0 && y2 >= 0)
            {
                var line = new Line() { X1 = 0, Y1 = 0, X2 = x2-x1, Y2 = y2-y1, StrokeThickness = thickness, Stroke = brush, Fill = brush };
                AddToCanvasAt(canvas, line, x1, y1); 
            }
        }

        public static void AddTriangle(Canvas canvas)
        {
            var polygon = new Polygon() { StrokeThickness = 1,  Stroke = Brushes.Black, Fill = Brushes.DarkBlue };
            polygon.Points.Add(new Point(0  ,100));
            polygon.Points.Add(new Point(100,100));
            polygon.Points.Add(new Point(50 ,0  ));
            polygon.Points.Add(new Point(0  ,100));
            AddToCanvasAt(canvas, polygon, 200, 50);
        }

        public static void AddRectangle(Canvas canvas)
        {
            var polygon = new Polygon() { StrokeThickness = 1,  Stroke = Brushes.Black, Fill = Brushes.DarkRed };
            polygon.Points.Add(new Point(0  ,100));
            polygon.Points.Add(new Point(100,100));
            polygon.Points.Add(new Point(100,  0));
            polygon.Points.Add(new Point(0  ,  0));
            AddToCanvasAt(canvas, polygon, 350, 50);
        }

        public static void AddCircleAt(Canvas canvas, double x, double y, double radius, Brush brush)
        {
            var circle = new Ellipse() { Width = radius, Height = radius, StrokeThickness = 1,  Stroke = brush, Fill = brush };
            AddToCanvasAt(canvas, circle, x, y);
        }

        public static void AddToCanvasAt(Canvas canvas, Shape shape, double x, double y)
        {
            canvas.Children.Add(shape);
            Canvas.SetLeft(shape, x);
            Canvas.SetTop (shape, y);
        }
    }
}
