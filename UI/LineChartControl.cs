using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UI
{
    class LineChartControl
    {
        #region ------------- Properties ----------------------------------------------------------
        #endregion



        #region ------------- Fields --------------------------------------------------------------
        private Canvas _canvas;
        private Polyline _data;
        private double _x;
        private double _horizontalStretchFactor;
        private double _margin = 10.0;
        private Brush _descriptionLineBrush = Brushes.DarkBlue;
        private double _descriptionLineThickness = 1;
        private List<double> _valuesCollection;
        #endregion



        #region ------------- Init ----------------------------------------------------------------
        public LineChartControl(Canvas canvas)
        {
            _canvas = canvas;
            _valuesCollection = new List<double>();
            Clear();
        }
        #endregion



        #region ------------- Methods -------------------------------------------------------------
        public void AddValue(double value)
        {
            _valuesCollection.Add(value);

            // the values grow towards the upper end of the canvas, therefore nagative
            // the diagram origin is at the lower left corner
            var y = value * _canvas.Height / 100;
            var xCurrent = _x * _horizontalStretchFactor;

            _data.Points.Add(new System.Windows.Point(xCurrent, -y)); 

            _x++;
            xCurrent = _x * _horizontalStretchFactor;
            if (xCurrent > _canvas.Width - (2*_margin))
                ReScale();
        }

        public void Clear()
        {
            _valuesCollection.Clear();
            _data = new Polyline() { Stroke = Brushes.Black, FillRule = FillRule.Nonzero };
            Refresh();
        }

        public void Refresh()
        {
            _canvas.Children.Clear();
            DrawDescriptions();
            CreateLineSeries();
            _x = 0;
            _horizontalStretchFactor = 1.0;
        }
        #endregion



        #region ------------- Implementation ------------------------------------------------------
        private void CreateLineSeries()
        {
            AddToCanvasAt(_canvas, _data, _margin, _canvas.ActualHeight - _margin); // add to lower left corner
        }

        private void DrawDescriptions()
        {
            DrawXAxis();
            DrawYAxis();
        }

        private void DrawYAxis()
        {
            var x = _margin;
            AddLine(_canvas, x, _margin, x, _canvas.ActualHeight - _margin, _descriptionLineBrush, _descriptionLineThickness);
        }

        private void DrawXAxis()
        {
            var y = _canvas.ActualHeight - _margin;
            AddLine(_canvas, _margin, y, _canvas.ActualWidth - _margin, y, _descriptionLineBrush, _descriptionLineThickness);
        }

        private void ReScale()
        {
            _horizontalStretchFactor *= 0.5;

            _x = 0;
            _data.Points.Clear();
            foreach(var value in _valuesCollection)
            {
                var y = value * _canvas.Height / 100;
                var xCurrent = _x * _horizontalStretchFactor;
                _data.Points.Add(new System.Windows.Point(xCurrent * _horizontalStretchFactor, -y));
                _x++;
            }

            _x *= _horizontalStretchFactor;
            _canvas.Children.Clear();
            DrawDescriptions();
            CreateLineSeries();

        }

        private void AddPoint(Canvas canvas, double x, double y, Brush brush)
        {
            var line = new Line() { X1 = 0, Y1 = 0, X2 = 1, Y2 = 1, StrokeThickness = 1, Stroke = brush, Fill = brush };
            AddToCanvasAt(canvas, line, x, y); 
        }

        private void AddLine(Canvas canvas, double x1, double y1, double x2, double y2, Brush brush, double thickness)
        {
            if (x1 >= 0 && y1 >= 0 && x2 >= 0 && y2 >= 0)
            {
                var line = new Line() { X1 = 0, Y1 = 0, X2 = x2-x1, Y2 = y2-y1, StrokeThickness = thickness, Stroke = brush, Fill = brush };
                AddToCanvasAt(canvas, line, x1, y1); 
            }
        }

        private void AddToCanvasAt(Canvas canvas, Shape shape, double x, double y)
        {
            canvas.Children.Add(shape);
            Canvas.SetLeft(shape, x);
            Canvas.SetTop (shape, y);
        }
        #endregion
    }
}
