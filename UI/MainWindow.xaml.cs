using Abraham.WPFWindowLayoutManager;
using NeuralNetwork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UI
{
    public partial class MainWindow : Window
    {
        #region ------------- Fields --------------------------------------------------------------
        private string _trainingDataDirectory = "./TrainingData";
        private Network _network;
        private Image _currentTrainingImage = null;
        private Image _currentNetworkWeights = null;
        private DrawPad _drawPad;
        private WindowLayoutManager _layoutManager;
        private double _margin;
        private double _totalHeight;
        private List<Point> _outputNeuronPositions;
        private List<Point> _inputNeuronPositions;
        private List<List<Point>> _hiddenNeuronPositions;
        #endregion



        #region ------------- Init ----------------------------------------------------------------
        public MainWindow()
        {
            _layoutManager = new WindowLayoutManager(window: this, key: "MainWindow");
            InitializeComponent();
            _drawPad = new DrawPad(HandwritingCanvas);
            InitStructureDisplay();
            _network = new Network();
            TrainingDataInit();
            TrainingInit();
            StructureInit();
            TestInit();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetInitialButtonStates();
        }

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            _network.CancelTraining();
			_layoutManager.Save();
		}
        #endregion



        #region ------------- Implementation ------------------------------------------------------
        #region ------------- Init ----------------------------------------------------------------
        private void InitStructureDisplay()
        {
        }

        private void SetInitialButtonStates()
        {
            buttonStartTraining.IsEnabled = false;
            buttonSaveNetwork.IsEnabled = false;
            buttonStopTraining.IsEnabled = false;
            //_drawPad.IsEnabled = false;
        }
        #endregion
        #region ------------- Training data -------------------------------------------------------
        private void TrainingDataInit()
        {
        }

        private void buttonLoadTrainingData_Click(object sender, RoutedEventArgs e)
        {
            string messages = "";
            var success = _network.StartLoadingTrainingData(_trainingDataDirectory, OnLoadFinished, ref messages);
            if (!success)
            {
                MessageBox.Show(messages);
                return;
            }
            SetStatusBeforeLoading();
        }

        private void OnLoadFinished()
        {
            Dispatcher.Invoke(() =>
            {
                SetFieldsInTrainingDataBoxAfterLoad();
                SetStatusAfterLoad();
                InitNetworkStructureAfterLoad();
                InitTrainingAfterLoad();
            });
        }

        private void SetFieldsInTrainingDataBoxAfterLoad()
        {
            textBoxImageSize.Text = $"{_network.TrainingImageSize} x {_network.TrainingImageSize}";
            textBoxImageSize.IsEnabled = false;
            textBoxImageCount.Text = $"{_network.TrainingImageCount}";
            textBoxImageCount.IsEnabled = false;

            sliderTrainingImage.Minimum = 0;
            sliderTrainingImage.Maximum = _network.TrainingImageCount - 1;
            CurrentTrainingImage.Source = CreateImageFromBytes(_network.GetTrainingImageById(0));
        }

        private void SetStatusBeforeLoading()
        {
            labelStatus.Content = "loading training data...";
        }

        private void SetStatusAfterLoad()
        {
            labelStatus.Content = "Training data loaded.";
        }

        private void sliderTrainingImage_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int imageIndex = Convert.ToInt32(sliderTrainingImage.Value);
            CurrentTrainingImage.Source = CreateImageFromBytes(_network.GetTrainingImageById(imageIndex));
            CurrentTrainingImageNumber.Content = imageIndex.ToString();
        }

        private BitmapSource CreateImageFromBytes(byte[] bytes)
        {
            PixelFormat pf = PixelFormats.Gray8; // means one byte per pixel
            int width = 28;
            int height = 28;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;
            var bitmapSource = BitmapSource.Create(width, height, 96, 96, pf, null, bytes, rawStride);
            return bitmapSource;
        }
        #endregion
        #region ------------- Structure -----------------------------------------------------------
        private void StructureInit()
        {
        }

        private void InitNetworkStructureAfterLoad()
        {
            _network.NeuronsInInputLayer   = _network.TrainingImageSize * _network.TrainingImageSize;
            _network.NeuronsInHiddenLayers = 16;
            _network.HiddenLayersCount     = 3;
            _network.NeuronsInOutputLayer  = 10;
            _network.Initialize();

            textBoxNeuronsInInputLayer  .Text = _network.NeuronsInInputLayer  .ToString();
            textBoxHiddenLayers         .Text = _network.HiddenLayersCount    .ToString();
            textBoxNeuronsInHiddenLayers.Text = _network.NeuronsInHiddenLayers.ToString();
            textBoxNeuronsInOutputLayer .Text = _network.NeuronsInOutputLayer .ToString();

            DrawStructure();
        }

        private void buttonResetNetwork_Click(object sender, EventArgs e)
        {
            var inputLayersEntered = false;
            if (int.TryParse(textBoxNeuronsInInputLayer.Text, out int inputNeurons))
                inputLayersEntered  = true;

            var hiddenLayersEntered = false;
            if (int.TryParse(textBoxHiddenLayers.Text, out int hiddenLayers))
                hiddenLayersEntered = true;

            var neuronsInHiddenLayersEntered = false;
            if (int.TryParse(textBoxNeuronsInHiddenLayers.Text, out int neuronsInHiddenLayers))
                neuronsInHiddenLayersEntered = true;

            var neuronsInOutputLayerEntered = false;
            if (int.TryParse(textBoxNeuronsInOutputLayer.Text, out int neuronsInOutputLayer))
                neuronsInOutputLayerEntered = true;

            if (inputLayersEntered          ) _network.NeuronsInInputLayer   = inputNeurons;
            if (hiddenLayersEntered         ) _network.HiddenLayersCount     = hiddenLayers;
            if (neuronsInHiddenLayersEntered) _network.NeuronsInHiddenLayers = neuronsInHiddenLayers;
            if (neuronsInOutputLayerEntered ) _network.NeuronsInOutputLayer  = neuronsInOutputLayer;

            _network.Initialize();
            DrawStructure();
        }

        private void DrawStructure()
        {
            _margin = 0.1;
            _totalHeight = canvasStructure.Height * 0.8;

            canvasStructure.Children.Clear();
            DrawInputNeurons();
            DrawHiddenNeurons();
            DrawOutputNeurons();
        }

        private void DrawInputNeurons()
        {
            var x = canvasStructure.Width * _margin;
            var y = (canvasStructure.Height/2) - (_totalHeight/2);
            _inputNeuronPositions = DrawNCirclesVertically(x, y, _totalHeight, _network.NeuronsInInputLayer, Brushes.Black);
        }

        private void DrawHiddenNeurons()
        {
            var x = canvasStructure.Width * 3 * _margin;
            if (_network.HiddenLayersCount == 1)
                x = canvasStructure.Width / 2;

            var y = (canvasStructure.Height/2) - (_totalHeight/2);

            var totalWidthForHiddenNeurons = canvasStructure.Width - (4 * _margin * canvasStructure.Width);
            var horizontalIncrement = totalWidthForHiddenNeurons / _network.HiddenLayersCount;

            _hiddenNeuronPositions = new List<List<Point>>();

            for (int h=0; h < _network.HiddenLayersCount; h++)
            {
                var pointList = DrawNCirclesVertically(x + h*horizontalIncrement, y, _totalHeight, _network.NeuronsInHiddenLayers, Brushes.Black);
                _hiddenNeuronPositions.Add(pointList);

                if (h == 0)
                    DrawNLines(_inputNeuronPositions, pointList, Brushes.Black);
                else
                    DrawNLines(_hiddenNeuronPositions[h-1], _hiddenNeuronPositions[h], Brushes.Black);
            }
        }

        private void DrawOutputNeurons()
        {
            var x = (1 - _margin) * canvasStructure.Width;
            var y = (canvasStructure.Height / 2) - (_totalHeight / 2);
            _outputNeuronPositions = DrawNCirclesVertically(x, y, _totalHeight, _network.NeuronsInOutputLayer, Brushes.Black);

            DrawNLines(_hiddenNeuronPositions[_network.HiddenLayersCount-1], _outputNeuronPositions, Brushes.Black);
        }

        private void DrawNLines(List<Point> from, List<Point> to, Brush brush)
        {
            foreach(var p2 in to)
                foreach(var p1 in from)
                     AddLine(canvasStructure, p1.X, p1.Y, p2.X, p2.Y, brush, 1);
        }

        private List<Point> DrawNCirclesVertically(double x, double y, double totalHeight, int count, Brush brush)
        {
            if (count == 1)
                y = y + (totalHeight / 2);

            var points = new List<Point>();
            var increment = (count > 1) ? totalHeight / (count - 1) : 0;
            for (int i = 0; i < count; i++)
            {
                var ypos = y + (i * increment);
                AddCircleAt(canvasStructure, x, ypos, 10, brush);
                points.Add(new Point(x+5, ypos+5));
            }
            return points;
        }

        private void AddLine(Canvas canvas, double x1, double y1, double x2, double y2, Brush brush, double thickness)
        {
            if (x1 >= 0 && y1 >= 0 && x2 >= 0 && y2 >= 0)
            {
                var line = new Line() { X1 = 0, Y1 = 0, X2 = x2-x1, Y2 = y2-y1, StrokeThickness = thickness, Stroke = brush, Fill = brush };
                AddToCanvasAt(canvas, line, x1, y1); 
            }
        }

        private void AddTriangle(Canvas canvas)
        {
            var polygon = new Polygon() { StrokeThickness = 1,  Stroke = Brushes.Black, Fill = Brushes.DarkBlue };
            polygon.Points.Add(new Point(0  ,100));
            polygon.Points.Add(new Point(100,100));
            polygon.Points.Add(new Point(50 ,0  ));
            polygon.Points.Add(new Point(0  ,100));
            AddToCanvasAt(canvas, polygon, 200, 50);
        }

        private void AddRectangle(Canvas canvas)
        {
            var polygon = new Polygon() { StrokeThickness = 1,  Stroke = Brushes.Black, Fill = Brushes.DarkRed };
            polygon.Points.Add(new Point(0  ,100));
            polygon.Points.Add(new Point(100,100));
            polygon.Points.Add(new Point(100,  0));
            polygon.Points.Add(new Point(0  ,  0));
            AddToCanvasAt(canvas, polygon, 350, 50);
        }

        private void AddCircleAt(Canvas canvas, double x, double y, double radius, Brush brush)
        {
            var circle = new Ellipse() { Width = radius, Height = radius, StrokeThickness = 1,  Stroke = brush, Fill = brush };
            AddToCanvasAt(canvas, circle, x, y);
        }

        private void AddToCanvasAt(Canvas canvas, Shape shape, double x, double y)
        {
            canvas.Children.Add(shape);
            Canvas.SetLeft(shape, x);
            Canvas.SetTop (shape, y);
        }
        #endregion
        #region ------------- Training ------------------------------------------------------------
        private void TrainingInit()
        {
            textBoxTrainingSpeed.Text = _network.TrainingSpeed.ToString();
        }

        private void buttonCheck_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonLoadNetwork_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonSaveNetwork_Click(object sender, RoutedEventArgs e)
        {

        }

        private void train_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonStopTraining_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion
        #region ------------- Test ----------------------------------------------------------------
        private void TestInit()
        {
        }
        #endregion
        #region ------------- Training ------------------------------------------------------------
        private void InitTrainingAfterLoad()
        {
            textBoxNeuronsInInputLayer.Text = _network.NeuronsInInputLayer.ToString();
            textBoxStopAfterIterations.Text = "60000";
            buttonStartTraining.IsEnabled = true;
            buttonSaveNetwork.IsEnabled = true;
        }

        private void buttonStartTraining_Click(object sender, EventArgs e)
        {
            var hiddenLayersEntered = false;
            if (int.TryParse(textBoxHiddenLayers.Text, out int hiddenLayers))
                hiddenLayersEntered = true;

            var neuronsInHiddenLayersEntered = false;
            if (int.TryParse(textBoxNeuronsInHiddenLayers.Text, out int neuronsInHiddenLayers))
                neuronsInHiddenLayersEntered = true;

            var stopAfterIterationsEntered = false;
            if (int.TryParse(textBoxStopAfterIterations.Text, out int iterations))
                stopAfterIterationsEntered = true;

            var stopAfterAccurracyEntered = false;
            if (int.TryParse(textBoxStopIfAccuracyIs.Text, out int accuracy))
                stopAfterAccurracyEntered = true;

            var trainingSpeedEntered = false;
            if (float.TryParse(textBoxTrainingSpeed.Text, out float trainingSpeed))
                trainingSpeedEntered = true;

            if (!ValidateEntries(
                hiddenLayersEntered, hiddenLayers,
                neuronsInHiddenLayersEntered, neuronsInHiddenLayers,
                stopAfterIterationsEntered, iterations,
                stopAfterAccurracyEntered, accuracy,
                trainingSpeedEntered, trainingSpeed,
                out string messages))
            {
                MessageBox.Show(messages);
                return;
            }

            _network.StopAfterIterations = 0;
            _network.StopAfterAccurracy = 0;
            if (hiddenLayersEntered) _network.HiddenLayersCount = hiddenLayers;
            if (neuronsInHiddenLayersEntered) _network.NeuronsInHiddenLayers = neuronsInHiddenLayers;
            if (stopAfterIterationsEntered) _network.StopAfterIterations = iterations;
            if (stopAfterAccurracyEntered) _network.StopAfterAccurracy = accuracy;
            if (trainingSpeedEntered) _network.TrainingSpeed = trainingSpeed;

            SetButtonsForTrainingProgress();

            _network.StartTraining(onTrainingProgress, onTrainingFinished);
        }

        private bool ValidateEntries(
            bool hiddenLayersEntered, int hiddenLayers,
            bool neuronsInHiddenLayersEntered, int neuronsInHiddenLayers,
            bool iterationsEntered, int iterations,
            bool accurracyEntered, int accuracy,
            bool trainingSpeedEntered, float trainingSpeed,
            out string messages)
        {
            messages = "";
            if (!iterationsEntered && !accurracyEntered)
            {
                messages = "please enter either iterations or accuracy, to set when training should end!";
                return false;
            }
            if (iterationsEntered && accurracyEntered)
            {
                messages = "please enter either iterations or accuracy, not both!";
                return false;
            }

            if (hiddenLayersEntered && hiddenLayers <= 0)
            {
                messages = "please enter the number of hidden layers as a positive number!";
                return false;
            }
            if (neuronsInHiddenLayersEntered && neuronsInHiddenLayers <= 0)
            {
                messages = "please enter the number of hidden layers as a positive number!";
                return false;
            }
            if (iterationsEntered && iterations <= 0)
            {
                messages = "please enter the number of iterations as a positive number!";
                return false;
            }
            if (accurracyEntered && (accuracy < 10 || accuracy > 100))
            {
                messages = "please enter accuracy threshold as a percentage between 10 and 100!";
                return false;
            }
            if (trainingSpeedEntered && (trainingSpeed <= 0 || trainingSpeed > 1.0))
            {
                messages = "please enter training speed as a number between 0 and 1!";
                return false;
            }
            return true;
        }

        private void onTrainingProgress(float[] currentOutput, string statusText, byte[] currentTrainingImage)
        {
            Dispatcher.Invoke(() =>
            {
                //_currentTrainingImage = BitmapConverter.ByteToBitmap(currentTrainingImage, _network.ImageSize, _network.ImageSize);
                //this.CurrentTrainingImage.Source = _currentTrainingImage;
                labelStatus.Content = statusText;
                ShowCurrentImageClassification(currentOutput);
            });
        }

        public void ShowCurrentImageClassification(float[] output)
        {
            var result = "";
            for (int i = 0; i < output.Length; i++)
            {
                result += $"{i}: {output[i].ToString("0.00")}\n";
            }
            labelTrainingClassification.Content = result;
        }

        private void onTrainingFinished(string statusText)
        {
            Dispatcher.Invoke(() =>
            {
                labelStatus.Content = statusText;
                SetButtonsAfterTraining();
            });
        }

        private void buttonCancelTraining_Click(object sender, EventArgs e)
        {
            _network.CancelTraining();
            labelStatus.Content = "training cancelled";
            SetButtonsAfterTraining();
        }

        private void SetButtonsForTrainingProgress()
        {
            textBoxNeuronsInHiddenLayers.IsEnabled = false;
            textBoxHiddenLayers.IsEnabled = false;
            textBoxStopAfterIterations.IsEnabled = false;
            textBoxStopIfAccuracyIs.IsEnabled = false;
            textBoxTrainingSpeed.IsEnabled = false;
            buttonStopTraining.IsEnabled = true;
            buttonStartTraining.IsEnabled = false;
            buttonLoadTrainingData.IsEnabled = false;
            labelStatus.Content = "Training ...";
        }

        private void SetButtonsAfterTraining()
        {
            _drawPad.OnUserHasDrawnAnImage = OnUserHasDrawnAnImage;
            //_drawPad.IsEnabled = true;
            //textBoxNeuronsInHiddenLayers.IsEnabled = true;
            //textBoxHiddenLayers         .IsEnabled = true;
            textBoxStopAfterIterations.IsEnabled = true;
            textBoxStopIfAccuracyIs.IsEnabled = true;
            textBoxTrainingSpeed.IsEnabled = true;
            buttonStopTraining.IsEnabled = false;
            buttonStartTraining.IsEnabled = true;
            buttonLoadTrainingData.IsEnabled = true;
        }
        #endregion
        #region ------------- Loading/saving ------------------------------------------------------
        private void buttonSaveNetwork_Click(object sender, EventArgs e)
        {
            _network.SaveNetwork(_trainingDataDirectory);
            labelStatus.Content = "Network saved.";
            //_drawPad.IsEnabled = true;
        }

        private void buttonLoadNetwork_Click(object sender, EventArgs e)
        {
            _network.LoadNetwork(_trainingDataDirectory);
            labelStatus.Content = "Network loaded.";
            //_drawPad.IsEnabled = true;
            _drawPad.OnUserHasDrawnAnImage = OnUserHasDrawnAnImage;
        }
        #endregion
        #region ------------- Checking handwritten image ------------------------------------------
        private void InitHandwritingDrawPad()
        {
            //_drawPad = new DrawPad();
            //_drawPad.Location = new Point(labelHandwritingInput.Left, 50);
            //_drawPad.Size = new Size(600, 600);
            //_drawPad.ImageSize = new Size(600, 600);
            //_drawPad.LineWidth = 25;
            //_drawPad.BackColor = Color.Black;
            //Controls.Add(_drawPad);
        }

        private void OnUserHasDrawnAnImage()
        {
            buttonCheck_Click(null, null);
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
//            var inputValues = GetHandWrittenImage();
//            if (inputValues is null)
//                return;

//            _currentTrainingImage = Bitmap.FromFile("img.bmp") as Bitmap;
//            this.currentTrainingImage.Image = _currentTrainingImage.Clone() as Bitmap;
//            _currentTrainingImage = null;
            //Refresh();

            //var outputNeurons = _network.Think(inputValues);

            //DisplayClassification(outputNeurons);
        }

//       private float[] GetHandWrittenImage()
//       {
//           _drawPad.RescaleImage(_network.ImageSize, _network.ImageSize);
//           _drawPad.CenterImage();
//           _drawPad.Image.Save(@"img.png", System.Drawing.Imaging.ImageFormat.Png);
//           _drawPad.Image.Save(@"img.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
//           float[] handwrittenImage = _drawPad.ImageToFloat();
//           return handwrittenImage;
//       }

        private void DisplayClassification(float[] outputNeurons)
        {
            var results = "";
            for (int i = 0; i < outputNeurons.Length; i++)
                results += i + ": " + outputNeurons[i].ToString("0.00") + "\n";

            labelClassicationResults.Content = results;
            labelClassification.Content = _network.GetOutputClassification(outputNeurons).ToString();
            _drawPad.ResetImage();
        }

        private void Handwriting_MouseLeave(object sender, MouseEventArgs e)
        {
            var bitmap = GetBitmapFromCanvas(HandwritingCanvas, 28, 28);
            //var stream = SaveBitmapToMemoryStream(bitmap);
            CurrentTrainingImage.Source = bitmap;

            //SaveBitmapToFile(bitmap, "handwriting.bmp");
            //var converter = new ImageSourceConverter();
            //var image = converter.ConvertFromString("handwriting.bmp") as ImageSource;
            //CurrentTrainingImage.Source = image;
            //File.Delete("handwriting.bmp");
        }

        private RenderTargetBitmap GetBitmapFromCanvas(Canvas canvas, int width, int height)
        {
            var renderBitmap = new RenderTargetBitmap(width, height, 1/300, 1/300, PixelFormats.Pbgra32);

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(canvas);
                context.DrawRectangle(brush, null, new Rect(new Point(), new Size(canvas.Width, canvas.Height)));
            }
            visual.Transform = new ScaleTransform(width / canvas.ActualWidth, height / canvas.ActualHeight);
            renderBitmap.Render(visual);
            return renderBitmap;
        }

        private void SaveBitmapToMemoryStream(RenderTargetBitmap renderBitmap, string path)
        {
            using (MemoryStream fs = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                //BitmapEncoder encoder = new JpegBitmapEncoder();
                //BitmapEncoder encoder = new TiffBitmapEncoder();
                //BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(fs);
                fs.Flush();
                fs.Close();
            }
        }    

        private void SaveBitmapToFile(RenderTargetBitmap renderBitmap, string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                //BitmapEncoder encoder = new JpegBitmapEncoder();
                //BitmapEncoder encoder = new TiffBitmapEncoder();
                //BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(fs);
                fs.Flush();
                fs.Close();
            }
        }
        #endregion
        #endregion
    }
}
