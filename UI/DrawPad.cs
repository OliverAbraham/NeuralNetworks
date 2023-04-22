using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UI
{
    class DrawPad
    {
        #region ------------- Properties ----------------------------------------------------------
        public Image  MyImage               { get; set; }
        public Image  DisplayImage          { get; set; }
        public Size   ImageSize             { get => _imageSize; set => OnImageResize(value); }
        public int    LineWidth             { get; set; }
        public bool   AlreadyDrawn          { get; set; }
        public bool   UserHasDrawn          { get; set; }
        public Action OnUserHasDrawnAnImage { get; set; }
        #endregion



        #region ------------- Fields --------------------------------------------------------------
        private Canvas _canvas;
        private Size _imageSize;
        private System.Windows.Point? prevPoint = null;
        private Brush _brush = Brushes.White;
        #endregion



        #region ------------- Init ----------------------------------------------------------------
        public DrawPad(Canvas canvas)
        {
            _canvas = canvas;
            _canvas.Background = Brushes.Black;
            this._imageSize = new Size(10, 10);
            this.LineWidth = 10;
            this.AlreadyDrawn = false;
            this.ResetImage();
            _canvas.MouseMove  += OnMouseMove;
            _canvas.MouseUp    += OnMouseUp;
            _canvas.MouseDown  += OnMouseDown;
            _canvas.MouseLeave += OnMouseLeave;
            UserHasDrawn = false;
        }
        #endregion



        #region ------------- Methods -------------------------------------------------------------
        public void ResetImage()
        {
            //this.MyImage = new Image(_imageSize.Width, _imageSize.Height);
            //this.AlreadyDrawn = false;
            //this.DisplayImage = this.MyImage;
            //this.Refresh();
        }
        #endregion



        #region ------------- Implementation ------------------------------------------------------
        private void OnImageResize(Size _size)
        {
            _imageSize = _size;
            this.ResetImage();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MyImage = DisplayImage;
                var imgPoint = e.GetPosition(_canvas);

                //if (prevPoint != null)
                //    DrawLine(new Pen(Brushes.Black, LineWidth * 2), (System.Windows.Point)prevPoint, imgPoint);
                DrawCircle(imgPoint.X, imgPoint.Y, LineWidth);

                DisplayImage = MyImage;
                prevPoint = imgPoint;
                AlreadyDrawn = true;
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            prevPoint = null;
            UserHasDrawn = true;
        }

        private void OnMouseLeave(object? sender, EventArgs e)
        {
            if (UserHasDrawn)
            {
                if (OnUserHasDrawnAnImage is not null)
                    OnUserHasDrawnAnImage();
                UserHasDrawn = false;
            }
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var imgPoint = e.GetPosition(_canvas);
                DrawCircle(imgPoint.X, imgPoint.Y, LineWidth);
            }
        }

        private void DrawCircle(double x, double y, int radius)
        {
            var ellipse = new Ellipse {  Height = radius*2, Width = radius*2, Fill = _brush };
            _canvas.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, x);
			Canvas.SetTop (ellipse, y);
        }
        #endregion
    }
}
